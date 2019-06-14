using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LowLevel_IO.PInvoke
{
    public static class Kernel32
    {
        public const uint GenericRead = ((uint)1 << 31);
        public const uint GenericWrite = ((uint)1 << 30);
        public const uint GenericAll = ((uint)1 << 28);
        public const uint FileShareRead = 1;
        public const uint Filesharewrite = 2;
        public const uint OpenExisting = 3;
        public const uint IoctlVolumeGetVolumeDiskExtents = 0x560000;
        public const uint IncorrectFunction = 1;
        public const uint ErrorInsufficientBuffer = 122;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);


        //https://docs.microsoft.com/en-us/windows/desktop/api/fileapi/nf-fileapi-createfilea
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetVolumeInformationByHandleW(
            IntPtr hDisk,
            StringBuilder volumeNameBuffer,
            int volumeNameSize,
            ref uint volumeSerialNumber,
            ref uint maximumComponentLength,
            ref uint fileSystemFlags,
            StringBuilder fileSystemNameBuffer,
            int nFileSystemNameSize);
    }
}
