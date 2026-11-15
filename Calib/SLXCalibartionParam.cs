using System;
using System.IO;
using Newtonsoft.Json;

namespace Calib
{
    [Serializable]
    public class SingleCameraParams
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double M11 { get; set; }
        public double M12 { get; set; }
        public double M21 { get; set; }
        public double K1 { get; set; }
        public double K2 { get; set; }
        public double Uc { get; set; }

        [JsonIgnore] public double D => A * CommonA + B * CommonB;
        [JsonIgnore] public double c => -B * CommonA + A * CommonB;
        [JsonIgnore] public double CommonA { get; set; }
        [JsonIgnore] public double CommonB { get; set; }
    }

    [Serializable]
    public class SLXCalibrationResult
    {
        public double A { get; set; } // 公共运动参数 a
        public double B { get; set; } // 公共运动参数 b
        public SingleCameraParams[] Cameras { get; set; } = new SingleCameraParams[3];

        public static void SaveToFile(SLXCalibrationResult result, string filePath)
        {
            string json = JsonConvert.SerializeObject(result, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static SLXCalibrationResult LoadFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<SLXCalibrationResult>(json);

            if (result?.Cameras != null)
            {
                foreach (var cam in result.Cameras)
                {
                    if (cam != null)
                    {
                        cam.CommonA = result.A;
                        cam.CommonB = result.B;
                    }
                }
            }
            return result;
        }
    }
}