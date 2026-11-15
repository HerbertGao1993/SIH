using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using OpenCvSharp;

namespace Calib
{
    public static class SLXCalibrator
    {
        private class InitialParams
        {
            public double A, B, C, D, m11, m12, m21, a0, b0, uc0;
        }

        // 1. 线性初值求解
        private static InitialParams SolveSingleCameraInit(Point2d[] xy, Point2d[] uv)
        {
            int n = xy.Length;
            var M = Matrix<double>.Build.Dense(n, 3);
            var Y = Vector<double>.Build.Dense(n, -1.0);
            for (int i = 0; i < n; i++)
            {
                M[i, 0] = xy[i].X;
                M[i, 1] = xy[i].Y;
                M[i, 2] = uv[i].Y;
            }
            var X = M.QR().Solve(Y);
            double rawA = X[0], rawB = X[1], rawD = X[2];

            double norm = Math.Sqrt(rawA * rawA + rawB * rawB);
            double A = rawA / norm;
            double B = rawB / norm;
            double D = rawD / norm;
            double C = 1.0 / norm;

            double P0x = -C * A, P0y = -C * B;
            double dx = -B, dy = A;

            var S0 = new double[n];
            for (int i = 0; i < n; i++)
                S0[i] = dx * (xy[i].X - P0x) + dy * (xy[i].Y - P0y);

            var M2 = Matrix<double>.Build.Dense(n, 3);
            var Y2 = Vector<double>.Build.Dense(n);
            for (int i = 0; i < n; i++)
            {
                M2[i, 0] = S0[i];
                M2[i, 1] = 1.0;
                M2[i, 2] = -uv[i].X * S0[i];
                Y2[i] = uv[i].X;
            }
            var X2 = M2.QR().Solve(Y2);

            return new InitialParams
            {
                A = A,
                B = B,
                C = C,
                D = D,
                m11 = X2[0],
                m12 = X2[1],
                m21 = X2[2],
                a0 = D * A,
                b0 = D * B,
                uc0 = 2048.0
            };
        }

        // 2. 残差计算
        private static double[] CalcResiduals(double[] p, Point2d[][] xy, Point2d[][] uv, int numCams)
        {
            double a = p[0], b = p[1];
            int totalPoints = xy.Sum(arr => arr.Length);
            var res = new double[totalPoints * 2];
            int idx = 0;

            for (int i = 0; i < numCams; i++)
            {
                int offset = 2 + i * 8;
                double th = p[offset], C = p[offset + 1];
                double m11 = p[offset + 2], m12 = p[offset + 3], m21 = p[offset + 4];
                double k1 = p[offset + 5], k2 = p[offset + 6], uc = p[offset + 7];

                double A = Math.Cos(th), B = Math.Sin(th);

                for (int j = 0; j < xy[i].Length; j++)
                {
                    double px = xy[i][j].X, py = xy[i][j].Y;
                    double u = uv[i][j].X, v = uv[i][j].Y;

                    double r_line = A * px + B * py + C + v * (A * a + B * b);
                    double s = A * py - B * px + v * (-B * a + A * b);
                    double u_id = (m11 * s + m12) / (m21 * s + 1.0);
                    double u_obs = u_id + k1 * Math.Pow(u_id - uc, 3) + k2 * Math.Pow(u_id - uc, 5);

                    res[idx++] = u_obs - u;      // 像素残差
                    res[idx++] = r_line;         // 直线约束残差
                }
            }
            return res;
        }

        // 3. 手动实现的 LM 优化器
        private static double[] OptimizeLM(double[] p0, Point2d[][] xy, Point2d[][] uv, bool optLine, int numCams)
        {
            int n = p0.Length;
            double[] p = (double[])p0.Clone();
            double[] r = CalcResiduals(p, xy, uv, numCams);
            int m = r.Length;

            double error = r.Sum(x => x * x);
            double lambda = 1e-3; // 初始阻尼系数
            double[] dp = new double[n];

            for (int iter = 0; iter < 100; iter++)
            {
                // 计算雅可比矩阵 J (m x n)
                var J = new double[m, n];
                for (int j = 0; j < n; j++)
                {
                    // 【修复Bug】：锁定直线参数 theta 和 C
                    if (!optLine && j >= 2)
                    {
                        int k = (j - 2) % 8;
                        if (k == 0 || k == 1) continue; // 锁定 theta 和 C
                    }

                    double step = 1.492e-8 * Math.Max(1.0, Math.Abs(p[j]));

                    p[j] += step;
                    double[] rPlus = CalcResiduals(p, xy, uv, numCams);
                    p[j] -= 2 * step;
                    double[] rMinus = CalcResiduals(p, xy, uv, numCams);
                    p[j] += step; // 复原

                    for (int i = 0; i < m; i++)
                        J[i, j] = (rPlus[i] - rMinus[i]) / (2 * step);
                }

                // 构建 H = J^T * J 和 g = -J^T * r
                var H = new double[n, n];
                var g = new double[n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < m; k++) sum += J[k, i] * J[k, j];
                        H[i, j] = sum;
                        H[j, i] = sum;
                    }
                    double gsum = 0;
                    for (int k = 0; k < m; k++) gsum -= J[k, i] * r[k];
                    g[i] = gsum;
                }

                bool stepAccepted = false;
                while (!stepAccepted && lambda < 1e12)
                {
                    // 求解 (H + lambda * diag(H)) * dp = g
                    // 为了防止锁定的参数导致奇异，加入微小的单位阵对角项
                    var Hdamp = Matrix<double>.Build.Dense(n, n, (i, j) => i == j ? H[i, j] * (1.0 + lambda) + 1e-12 : H[i, j]);
                    var gVec = Vector<double>.Build.Dense(g);

                    try
                    {
                        var dpVec = Hdamp.QR().Solve(gVec);
                        for (int i = 0; i < n; i++) dp[i] = dpVec[i];
                    }
                    catch
                    {
                        lambda *= 10;
                        continue;
                    }

                    // 尝试新参数
                    for (int i = 0; i < n; i++) p[i] += dp[i];
                    double[] rNew = CalcResiduals(p, xy, uv, numCams);
                    double errorNew = rNew.Sum(x => x * x);

                    if (errorNew < error)
                    {
                        // 接受更新
                        r = rNew;
                        error = errorNew;
                        lambda = Math.Max(lambda / 10.0, 1e-9); // 缩小阻尼
                        stepAccepted = true;
                    }
                    else
                    {
                        // 拒绝更新，恢复参数，增大阻尼
                        for (int i = 0; i < n; i++) p[i] -= dp[i];
                        lambda *= 10;
                    }
                }

                if (lambda >= 1e12) break; // 已无法收敛
            }
            return p;
        }

        /// <summary>
        /// 多相机联合标定
        /// </summary>
        public static SLXCalibrationResult Calibrate(List<Point2d>[] xyLists, List<Point2d>[] uvLists, bool optimizeLine, out string mes)
        {
            int numCams = xyLists.Length;
            if (numCams > 3) throw new ArgumentException("最多支持 3 台相机");

            var xy = xyLists.Select(x => x.ToArray()).ToArray();
            var uv = uvLists.Select(x => x.ToArray()).ToArray();

            // 1. 求各相机线性初值
            var inits = new InitialParams[numCams];
            for (int i = 0; i < numCams; i++)
                inits[i] = SolveSingleCameraInit(xy[i], uv[i]);

            // 2. 组装全局参数初值 p0
            int paramCount = 2 + numCams * 8;
            var p0 = new double[paramCount];

            p0[0] = inits.Average(init => init.a0);
            p0[1] = inits.Average(init => init.b0);

            for (int i = 0; i < numCams; i++)
            {
                int offset = 2 + i * 8;
                p0[offset + 0] = Math.Atan2(inits[i].B, inits[i].A); // theta
                p0[offset + 1] = inits[i].C;
                p0[offset + 2] = inits[i].m11;
                p0[offset + 3] = inits[i].m12;
                p0[offset + 4] = inits[i].m21;
                p0[offset + 5] = 0.0;  // k1
                p0[offset + 6] = 0.0;  // k2
                p0[offset + 7] = inits[i].uc0;
            }

            // 3. 执行 LM 优化
            var pOpt = OptimizeLM(p0, xy, uv, optimizeLine, numCams);

            // 4. 打包结果
            var finalResult = new SLXCalibrationResult { A = pOpt[0], B = pOpt[1] };
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"公共运动向量: a = {finalResult.A:F6}, b = {finalResult.B:F6}");
            sb.AppendLine("==================================================");

            for (int i = 0; i < numCams; i++)
            {
                int offset = 2 + i * 8;
                var cam = new SingleCameraParams
                {
                    A = Math.Cos(pOpt[offset]),
                    B = Math.Sin(pOpt[offset]),
                    C = pOpt[offset + 1],
                    M11 = pOpt[offset + 2],
                    M12 = pOpt[offset + 3],
                    M21 = pOpt[offset + 4],
                    K1 = pOpt[offset + 5],
                    K2 = pOpt[offset + 6],
                    Uc = pOpt[offset + 7],
                    CommonA = finalResult.A,
                    CommonB = finalResult.B
                };
                finalResult.Cameras[i] = cam;

                CalculateErrors(cam, xy[i], uv[i], out double uvRms, out double uvMax, out double xyRms, out double xyMax);
                sb.AppendLine($"相机 {i + 1}: UV RMS={uvRms:F4}px, Max={uvMax:F4}px | XY RMS={xyRms:F4}mm, Max={xyMax:F4}mm");
            }

            mes = sb.ToString();
            Console.WriteLine(mes);
            return finalResult;
        }

        private static void CalculateErrors(SingleCameraParams p, Point2d[] xy, Point2d[] uv, out double uvRms, out double uvMax, out double xyRms, out double xyMax)
        {
            int n = xy.Length;
            double[] u_res = new double[n], v_res = new double[n], xy_err = new double[n];

            for (int i = 0; i < n; i++)
            {
                double x = xy[i].X, y = xy[i].Y, u = uv[i].X, v = uv[i].Y;

                // 正投影 (【修复Bug】：用观测的 v 算 s 和 u)
                double s = p.A * y - p.B * x + v * p.c;
                double u_id = (p.M11 * s + p.M12) / (p.M21 * s + 1.0);
                double u_pred = u_id + p.K1 * Math.Pow(u_id - p.Uc, 3) + p.K2 * Math.Pow(u_id - p.Uc, 5);
                u_res[i] = u_pred - u;

                double v_pred = -(p.A * x + p.B * y + p.C) / p.D;
                v_res[i] = v_pred - v;

                // 反投影（Newton 去畸变）
                double du = u - p.Uc;
                for (int k = 0; k < 5; k++)
                {
                    double g = du + p.K1 * Math.Pow(du, 3) + p.K2 * Math.Pow(du, 5);
                    double gp = 1 + 3 * p.K1 * Math.Pow(du, 2) + 5 * p.K2 * Math.Pow(du, 4);
                    du -= (g - (u - p.Uc)) / gp;
                }
                double u_id_val = p.Uc + du;
                double S_val = (p.M12 - u_id_val) / (u_id_val * p.M21 - p.M11);

                double xy_M_x = (-p.C * p.A) + S_val * (-p.B) - v * p.CommonA;
                double xy_M_y = (-p.C * p.B) + S_val * (p.A) - v * p.CommonB;

                xy_err[i] = Math.Sqrt(Math.Pow(xy_M_x - x, 2) + Math.Pow(xy_M_y - y, 2));
            }

            uvRms = Math.Sqrt(u_res.Zip(v_res, (a, b) => a * a + b * b).Average());
            uvMax = Math.Sqrt(u_res.Zip(v_res, (a, b) => a * a + b * b).Max());
            xyRms = Math.Sqrt(xy_err.Average(x => x * x));
            xyMax = xy_err.Max();
        }
    }
}