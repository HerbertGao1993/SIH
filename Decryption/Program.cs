using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenCvSharp;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Forms;

namespace Decryption
{
    class Program
    {
        static void Main(string[] args)
        {
            int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
            byte[] IV = System.Text.Encoding.ASCII.GetBytes(readIV());
            Console.WriteLine("Enter Password:");
            string pass = Console.ReadLine();
            while (pass.Length < 16)
            {
                pass += "\x0";
            }
            pass = pass.Substring(0, 16);
            byte[] key = System.Text.Encoding.ASCII.GetBytes(pass);
            FileStream fs2 = new FileStream(args[0], FileMode.Open);
            try
            {
                byte[] newfile = new byte[fs2.Length];
                fs2.Read(newfile, 0, newfile.Length);
                fs2.Close();
                pass = pass.Substring(0, 16);

                byte[] rawbytes = Decrypt(newfile, key, IV);
                Mat m = Cv2.ImDecode(rawbytes, ImreadModes.Color);
                double ratiow =  ScreenWidth * 1.0 / m.Cols * 1.0;
                double ratioh = ScreenHeight * 1.0 / m.Rows * 1.0;
                double ratio = 0.9 * Math.Min(ratiow, ratioh);

                Console.WriteLine(ScreenWidth + "," + ScreenHeight);
                Cv2.NamedWindow(args[0],WindowFlags.KeepRatio);
                Cv2.ResizeWindow(args[0], (int)(m.Cols* ratio), (int)(m.Rows * ratio));
                Cv2.MoveWindow(args[0], 0, 0);
                Console.WriteLine((int)(m.Cols * ratio) + "," + (int)(m.Rows * ratio));
                Cv2.ImShow(args[0], m);
                Cv2.WaitKey(0);
            }
            catch
            {

            }
            System.Environment.Exit(0);
            Process.GetCurrentProcess().Kill();
        }

        public static byte[] Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC; // 使用CBC模式
                aesAlg.Padding = PaddingMode.PKCS7; // 使用PKCS7填充

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                return decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            }
        }
        private static string readIV()
        {
            string registryPath = @"SOFTWARE\SIH";
            string keyname = "IV";
            string defaultvalue = "Herbert199391003";
            string value = "Herbert199391003";
            RegistryKey key = null;
            try
            {
                key = Registry.LocalMachine.OpenSubKey(registryPath, true);
                if (key == null)
                {
                    key = Registry.LocalMachine.CreateSubKey(registryPath, true);
                }
                if (key.GetValue(keyname) == null)
                {
                    key.SetValue(keyname, defaultvalue);
                }
                value = key.GetValue(keyname).ToString();
                while (value.Length < 16)
                {
                    value += "\x00";
                }
                value = value.Substring(0, 16);
            }
            catch { }
            finally
            {
                if (key != null)
                {
                    key.Dispose();
                }
            }
            return value;
        }
    }
}

