using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.LowLevel_IO.PInvoke
{
    public class WIN32_DiskDrive
    {
        public UInt16 Availability { get; set; }
        public UInt32 BytesPerSector { get; set; }
        public UInt16[] Capabilities { get; set; }
        public string[] CapabilityDescriptions { get; set; }
        public string Caption { get; set; }
        public string CompressionMethod { get; set; }
        public UInt32 ConfigManagerErrorCode { get; set; }
        public bool ConfigManagerUserConfig { get; set; }
        public string CreationClassName { get; set; }
        public UInt64 DefaultBlockSize { get; set; }
        public string Description { get; set; }
        public string DeviceID { get; set; }
        public bool ErrorCleared { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorMethodology { get; set; }
        public string FirmwareRevision { get; set; }
        public UInt32 Index { get; set; }
        public DateTime InstallDate { get; set; }
        public string InterfaceType { get; set; }
        public UInt32 LastErrorCode { get; set; }
        public string Manufacturer { get; set; }
        public UInt64 MaxBlockSize { get; set; }
        public UInt64 MaxMediaSize { get; set; }
        public bool MediaLoaded { get; set; }
        public string MediaType { get; set; }
        public UInt64 MinBlockSize { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public bool NeedsCleaning { get; set; }
        public UInt32 NumberOfMediaSupported { get; set; }
        public UInt32 Partitions { get; set; }
        public string PNPDeviceID { get; set; }
        public UInt16[] PowerManagementCapabilities { get; set; }
        public bool PowerManagementSupported { get; set; }
        public UInt32 SCSIBus { get; set; }
        public UInt16 SCSILogicalUnit { get; set; }
        public UInt16 SCSIPort { get; set; }
        public UInt16 SCSITargetId { get; set; }
        public UInt32 SectorsPerTrack { get; set; }
        public string SerialNumber { get; set; }
        public UInt32 Signature { get; set; }
        public UInt64 Size { get; set; }
        public string Status { get; set; }
        public UInt16 StatusInfo { get; set; }
        public string SystemCreationClassName { get; set; }
        public string SystemName { get; set; }
        public UInt64 TotalCylinders { get; set; }
        public UInt32 TotalHeads { get; set; }
        public UInt64 TotalSectors { get; set; }
        public UInt64 TotalTracks { get; set; }
        public UInt32 TracksPerCylinder { get; set; }

        public static WIN32_DiskDrive[] GetDrives()
        {
            List<WIN32_DiskDrive> result = new List<WIN32_DiskDrive>();

            var query = new WqlObjectQuery("SELECT * FROM Win32_DiskDrive");
            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject coll in searcher.Get())
                {
                    WIN32_DiskDrive temp = new WIN32_DiskDrive();

                    foreach (PropertyInfo pi in typeof(WIN32_DiskDrive).GetProperties())
                        pi.SetValue(temp, coll[pi.Name]);

                    result.Add(temp);
                }
            }
            return result.ToArray();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Caption, SizeString());
        }


        public string SizeString()
        {
            string[] postfix = new string[] { "B", "kB", "MB", "GB", "TB" };
            int p = 0;
            UInt64 size = Size;

            while (size > 1024)
            {
                size /= 1024;
                p++;
            }

            return string.Format("{0} {1}", size, postfix[p]);
        }
    }
}