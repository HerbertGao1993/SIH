using System;
using System.Linq;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Calib
{
    public class SLXStitcher
    {
        public static unsafe Mat Stitch(Mat[] images, SLXCalibrationResult result, double meshSize = 0.5)
        {
            int camCount = images.Length;
            if (camCount < 1 || camCount > 3) throw new ArgumentException("仅支持 1-3 台相机");

            // ================= 1. 直接通过四个角点计算所有相机的世界坐标边界 =================
            double globalXMin = double.MaxValue, globalXMax = double.MinValue;
            double globalYMin = double.MaxValue, globalYMax = double.MinValue;

            double[] camXMin = new double[camCount];
            double[] camXMax = new double[camCount];

            for (int i = 0; i < camCount; i++)
            {
                var p = result.Cameras[i];
                int w = images[i].Width, h = images[i].Height;
                double[] us = { 0, w - 1, 0, w - 1 };
                double[] vs = { 0, 0, h - 1, h - 1 };

                double currentXMin = double.MaxValue, currentXMax = double.MinValue;

                for (int k = 0; k < 4; k++)
                {
                    var worldPt = ImageToWorld(us[k], vs[k], p);
                    currentXMin = Math.Min(currentXMin, worldPt.X);
                    currentXMax = Math.Max(currentXMax, worldPt.X);
                    globalXMin = Math.Min(globalXMin, worldPt.X);
                    globalXMax = Math.Max(globalXMax, worldPt.X);
                    globalYMin = Math.Min(globalYMin, worldPt.Y);
                    globalYMax = Math.Max(globalYMax, worldPt.Y);
                }
                camXMin[i] = currentXMin;
                camXMax[i] = currentXMax;
            }

            int outW = (int)Math.Ceiling((globalXMax - globalXMin) / meshSize);
            int outH = (int)Math.Ceiling((globalYMax - globalYMin) / meshSize);

            // ================= 2. 相机排序并计算重叠区中线 =================
            // 按照相机的 X 物理重心从左到右排序
            var sortedIdx = Enumerable.Range(0, camCount)
                .OrderBy(i => (camXMin[i] + camXMax[i]) / 2.0)
                .ToArray();

            // 计算相邻相机的重叠中线 X_mid
            double[] xMids = new double[camCount - 1];
            for (int i = 0; i < camCount - 1; i++)
            {
                int leftIdx = sortedIdx[i];
                int rightIdx = sortedIdx[i + 1];

                // 左边的右边界 和 右边的左边界 的交集就是重叠区
                double overlapMin = Math.Max(camXMin[leftIdx], camXMin[rightIdx]);
                double overlapMax = Math.Min(camXMax[leftIdx], camXMax[rightIdx]);

                if (overlapMax > overlapMin)
                    xMids[i] = (overlapMin + overlapMax) / 2.0;
                else
                    xMids[i] = camXMax[leftIdx]; // 如果无重叠，直接以左相机边界为界
            }

            // ================= 3. 生成重采样图 =================
            Mat[] warped = new Mat[camCount];
            bool[][] masks = new bool[camCount][];
            for (int i = 0; i < camCount; i++)
            {
                masks[i] = new bool[outW * outH]; // 提前分配，解决 out 参数不能进 Lambda 的问题
                warped[i] = GenerateRemap(images[i], result.Cameras[i], globalXMin, globalYMin, meshSize, outW, outH, masks[i]);
            }

            Mat stitched = new Mat(outH, outW, MatType.CV_8UC1, Scalar.All(0));
            byte* ptrRes = (byte*)stitched.Data.ToPointer();
            int stepRes = (int)stitched.Step();

            byte** ptrWarps = stackalloc byte*[camCount];
            int[] warpSteps = new int[camCount];
            for (int i = 0; i < camCount; i++)
            {
                ptrWarps[i] = (byte*)warped[i].Data.ToPointer();
                warpSteps[i] = (int)warped[i].Step();
            }

            double blendHalfWidth = 5.0; // ±5mm 混合带

            // ================= 4. 核心融合 =================
            Parallel.For(0, outH, row =>
            {
                byte* rowRes = ptrRes + row * stepRes;
                for (int col = 0; col < outW; col++)
                {
                    double X = globalXMin + col * meshSize;
                    double sumVal = 0;
                    double sumWeight = 0;

                    for (int s = 0; s < camCount; s++)
                    {
                        int idx = sortedIdx[s];
                        int flatIdx = row * outW + col;
                        if (!masks[idx][flatIdx]) continue;

                        double weight = 1.0;

                        // 处理左侧融合（只有当它不是最左侧相机时）
                        if (s > 0)
                        {
                            double mid = xMids[s - 1];
                            if (X < mid - blendHalfWidth)
                                weight = 0.0; // 完全在左侧相机地盘
                            else if (X <= mid + blendHalfWidth)
                                weight = (X - (mid - blendHalfWidth)) / (2 * blendHalfWidth);
                        }

                        // 处理右侧融合（只有当它不是最右侧相机时）
                        if (s < camCount - 1)
                        {
                            double mid = xMids[s];
                            if (X > mid + blendHalfWidth)
                                weight = 0.0; // 完全在右侧相机地盘
                            else if (X >= mid - blendHalfWidth)
                            {
                                double w2 = ((mid + blendHalfWidth) - X) / (2 * blendHalfWidth);
                                weight = Math.Min(weight, w2); // 三相机交汇处取最小
                            }
                        }

                        if (weight > 1e-5)
                        {
                            byte* rowWarp = ptrWarps[idx] + row * warpSteps[idx];
                            sumVal += rowWarp[col] * weight;
                            sumWeight += weight;
                        }
                    }

                    if (sumWeight > 1e-5)
                    {
                        double finalVal = sumVal / sumWeight;
                        // 替代 .NET Core 的 Math.Clamp
                        finalVal = Math.Min(255.0, Math.Max(0.0, finalVal));
                        rowRes[col] = (byte)finalVal;
                    }
                }
            });

            foreach (var mat in warped) mat.Dispose();
            return stitched;
        }

        private static unsafe Mat GenerateRemap(Mat img, SingleCameraParams p,
            double xMin, double yMin, double mesh, int outW, int outH, bool[] mask)
        {
            Mat mapX = new Mat(outH, outW, MatType.CV_32FC1);
            Mat mapY = new Mat(outH, outW, MatType.CV_32FC1);

            float* px = (float*)mapX.Data.ToPointer();
            float* py = (float*)mapY.Data.ToPointer();
            int stepX = (int)(mapX.Step() / sizeof(float));
            int stepY = (int)(mapY.Step() / sizeof(float));

            // 预计算常量
            double theta = Math.Atan2(-p.CommonA, -p.CommonB);
            double cosT = Math.Cos(theta), sinT = Math.Sin(theta);
            double D = p.A * p.CommonA + p.B * p.CommonB;
            double cc = -p.B * p.CommonA + p.A * p.CommonB;

            double Kvx = (p.B * sinT - p.A * cosT) / D;
            double Kvy = -(p.A * sinT + p.B * cosT) / D;
            double Kvc = -p.C / D;

            double Ksx = -p.A * sinT - p.B * cosT + cc * Kvx;
            double Ksy = p.A * cosT - p.B * sinT + cc * Kvy;
            double Ksc = cc * Kvc;

            int w = img.Width, h = img.Height;

            Parallel.For(0, outH, i =>
            {
                double Y = yMin + i * mesh;
                float* rowX = px + i * stepX;
                float* rowY = py + i * stepY;

                for (int j = 0; j < outW; j++)
                {
                    double X = xMin + j * mesh;
                    double v = Kvx * X + Kvy * Y + Kvc;
                    double s = Ksx * X + Ksy * Y + Ksc;

                    double u_id = (p.M11 * s + p.M12) / (p.M21 * s + 1.0);
                    double du = u_id - p.Uc;
                    double du2 = du * du;
                    double u = u_id + du * du2 * (p.K1 + p.K2 * du2);

                    rowX[j] = (float)u;
                    rowY[j] = (float)v;

                    if (u >= 0 && u < w && v >= 0 && v < h)
                        mask[i * outW + j] = true;
                }
            });

            Mat warped = new Mat();
            Cv2.Remap(img, warped, mapX, mapY, InterpolationFlags.Linear, BorderTypes.Constant, Scalar.All(0));
            mapX.Dispose(); mapY.Dispose();
            return warped;
        }

        private static WorldPoint ImageToWorld(double u, double v, SingleCameraParams p)
        {
            double du = u - p.Uc;
            for (int k = 0; k < 5; k++)
            {
                double g = du + p.K1 * Math.Pow(du, 3) + p.K2 * Math.Pow(du, 5);
                double gp = 1 + 3 * p.K1 * Math.Pow(du, 2) + 5 * p.K2 * Math.Pow(du, 4);
                du -= (g - (u - p.Uc)) / gp;
            }
            double u_id = p.Uc + du;
            double s = (p.M12 - u_id) / (u_id * p.M21 - p.M11);
            double x_exp = -p.C * p.A + s * (-p.B);
            double y_exp = -p.C * p.B + s * p.A;

            double x_cal = x_exp - v * p.CommonA;
            double y_cal = y_exp - v * p.CommonB;

            double theta = Math.Atan2(-p.CommonA, -p.CommonB);
            double cosT = Math.Cos(theta), sinT = Math.Sin(theta);

            return new WorldPoint
            {
                X = x_cal * cosT - y_cal * sinT,
                Y = x_cal * sinT + y_cal * cosT
            };
        }

        // 辅助类，避免使用 C# 7.0 的 ValueTuple (兼容旧版 VS2015)
        private class WorldPoint
        {
            public double X { get; set; }
            public double Y { get; set; }
        }
    }
}