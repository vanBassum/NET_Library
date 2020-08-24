using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace FRMLib
{
    public static class Kernel32
    {
        public const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        public const int PROCESS_WM_READ = 0x0010;
        public const int PROCESS_WM_WRITE = 0x0020;
        public const int PROCESS_WM_OPERATION = 0x0008;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);


    }

    public class MemoryManager
    {
        Process process;
        IntPtr processHandle;

        #region ProcessAttach
        
        public bool attach(string processname)
        {
            try
            {
                process = Process.GetProcessesByName(processname)[0];
                processHandle = Kernel32.OpenProcess(Kernel32.PROCESS_WM_READ | Kernel32.PROCESS_WM_WRITE | Kernel32.PROCESS_WM_OPERATION, false, process.Id);
                return true;
            }
            catch
            {
                if (process != null)
                {
                    process.Close();
                    process.Dispose();
                }
            }

            return false;
        }

        public bool IsAttached()
        {
            if (process != null)
            {
                return true;
            }
            return false;
        }

        public bool detach()
        {
            try
            {
                if (process != null)
                {
                    process.Close();
                    process.Dispose();
                }
                return true;
            }
            catch
            {
            }

            return false;
        }



        #endregion

        public IntPtr GetProcessBase()
        {
            return process.MainModule.BaseAddress;       
        }

        public IntPtr GetProcessModuleBase(string moduleName)
        {
            var mod = process.Modules.Cast<ProcessModule>().Where(p => p.ModuleName == moduleName).FirstOrDefault();
            if (mod != null)
                return mod.BaseAddress;
            return IntPtr.Zero;
        }


        #region Read
        public byte[] Read_Bytes(IntPtr address, int length)
        {
            byte[] buffer = new byte[length];
            IntPtr bytesRead = IntPtr.Zero;
            Kernel32.ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);
            return buffer;
        }

        public IntPtr Read_Address(IntPtr address)
        {
            byte[] buffer = Read_Bytes(address, IntPtr.Size);
            if (IntPtr.Size == 4)
                return new IntPtr(Read_Int32(address));
            else
                return new IntPtr(Read_Int64(address));
        }

        public Int32 Read_Int32(IntPtr address)
        {
            byte[] buffer = Read_Bytes(address, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public Int64 Read_Int64(IntPtr address)
        {
            byte[] buffer = Read_Bytes(address, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        public float Read_Float(IntPtr address)
        {
            byte[] buffer = Read_Bytes(address, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        public double Read_Double(IntPtr address)
        {
            byte[] buffer = Read_Bytes(address, 4);
            return BitConverter.ToDouble(buffer, 0);
        }

        public string Read_String(IntPtr address, int strLength)
        {

            byte[] buffer = Read_Bytes(address, strLength);
            char[] chars = new char[strLength / sizeof(char)];
            System.Buffer.BlockCopy(buffer, 0, chars, 0, buffer.Length);
            return new string(chars);
        }

        public byte Read_Byte(IntPtr address)
        {
            byte[] buffer = Read_Bytes(address, 1);
            return buffer[0];
        }
        #endregion



    }

}
