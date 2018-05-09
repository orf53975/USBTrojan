using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;

namespace USBTrojan
{
    class HWID
    {
        private static string _fingerPrint = string.Empty;
        private const string HWIDCachePath = "hwid.cfg";
        public static string GetHWID()
        {
            if (File.Exists(HWIDCachePath)) _fingerPrint = File.ReadAllText(HWIDCachePath);
            if (string.IsNullOrEmpty(_fingerPrint))
            {
                _fingerPrint = GetHash("CPU >> " + CpuId() +
                    "\nBIOS >> " + BiosId() +
                    "\nBASE >> " + BaseId() +
                    "\nDISK >> " + DiskId() +
                    "\nVIDEO >> " + VideoId() +
                    "\nMAC >> " + MacId());
                File.WriteAllText(HWIDCachePath, _fingerPrint);
            }
            return _fingerPrint;
        }
        private static string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            byte[] bt = Encoding.ASCII.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }

        private static string GetHexString(IList<byte> bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Count; i++)
            {
                byte b = bt[i];
                int n = b;
                int n1 = n & 15;
                int n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + 'A')).ToString(CultureInfo.InvariantCulture);
                else
                    s += n2.ToString(CultureInfo.InvariantCulture);
                if (n1 > 9)
                    s += ((char)(n1 - 10 + 'A')).ToString(CultureInfo.InvariantCulture);
                else
                    s += n1.ToString(CultureInfo.InvariantCulture);
                if ((i + 1) != bt.Count && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }
        
        private static string Identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementBaseObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() != "True") continue;
                if (result != "") continue;
                try
                {
                    result = mo[wmiProperty].ToString();
                    break;
                }
                catch
                { }
            }
            return result;
        }
        private static string Identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementBaseObject mo in moc)
            {
                if (result != "") continue;
                try
                {
                    result = mo[wmiProperty].ToString();
                    break;
                }
                catch
                {
                }
            }
            return result;
        }
        private static string CpuId()
        {
            string retVal = Identifier("Win32_Processor", "UniqueId");
            if (retVal != "") return retVal;
            retVal = Identifier("Win32_Processor", "ProcessorId");
            if (retVal != "") return retVal;
            retVal = Identifier("Win32_Processor", "Name");
            if (retVal == "")
            {
                retVal = Identifier("Win32_Processor", "Manufacturer");
            }
            retVal += Identifier("Win32_Processor", "MaxClockSpeed");
            return retVal;
        }
        private static string BiosId()
        {
            return Identifier("Win32_BIOS", "Manufacturer") + Identifier("Win32_BIOS", "SMBIOSBIOSVersion") + Identifier("Win32_BIOS", "IdentificationCode") + Identifier("Win32_BIOS", "SerialNumber") + Identifier("Win32_BIOS", "ReleaseDate") + Identifier("Win32_BIOS", "Version");
        }
        private static string DiskId()
        {
            return Identifier("Win32_DiskDrive", "Model") + Identifier("Win32_DiskDrive", "Manufacturer") + Identifier("Win32_DiskDrive", "Signature") + Identifier("Win32_DiskDrive", "TotalHeads");
        }
        private static string BaseId()
        {
            return Identifier("Win32_BaseBoard", "Model") + Identifier("Win32_BaseBoard", "Manufacturer") + Identifier("Win32_BaseBoard", "Name") + Identifier("Win32_BaseBoard", "SerialNumber");
        }
        private static string VideoId()
        {
            return Identifier("Win32_VideoController", "DriverVersion") + Identifier("Win32_VideoController", "Name");
        }
        private static string MacId()
        {
            return Identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
        }
    }
}
