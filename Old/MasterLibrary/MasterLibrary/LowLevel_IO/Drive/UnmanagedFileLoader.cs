using MasterLibrary.LowLevel_IO.PInvoke;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.LowLevel_IO.Drive
{
    //https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.safehandles.safefilehandle?view=netframework-4.8
    class UnmanagedFileLoader
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

        private SafeFileHandle handleValue = null;


        public UnmanagedFileLoader(string Path)
        {
            Load(Path);
        }

        public void Load(string Path)
        {
            if (Path == null || Path.Length == 0)
            {
                throw new ArgumentNullException("Path");
            }

            // Try to open the file.
            handleValue = Kernel32.CreateFile(Path,
                GenericRead | GenericWrite,
                FileShareRead | Filesharewrite,
                IntPtr.Zero,
                OpenExisting,
                0,
                IntPtr.Zero);

            // If the handle is invalid,
            // get the last Win32 error 
            // and throw a Win32Exception.
            if (handleValue.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
        }

        public SafeFileHandle Handle
        {
            get
            {
                // If the handle is valid,
                // return it.
                if (!handleValue.IsInvalid)
                {
                    return handleValue;
                }
                else
                {
                    return null;
                }
            }

        }

    }
}
