using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Text;
using MasterLibrary.LowLevel_IO.PInvoke;
using MasterLibrary.LowLevel_IO.Streams;

namespace MasterLibrary.LowLevel_IO.Drive
{
    public class Disk : ISectorStream
    {
        Stream innerStream;
        UnmanagedFileLoader unmanagedFile;
        Int64 currentSector = 0;
        Int64 numberOfSectors = 0;
        Int32 bytesPerSector = 0;

        public long CurrentSector { get => currentSector; }
        public long NumberOfSectors { get => numberOfSectors; }
        public int BytesPerSector { get => bytesPerSector; }
        public bool CanRead => innerStream.CanRead;
        public bool CanSeek => innerStream.CanSeek;
        public bool CanWrite => innerStream.CanWrite;

        public void Open(WIN32_DiskDrive drive)
        {
            unmanagedFile = new UnmanagedFileLoader(drive.Name);
            innerStream = new FileStream(unmanagedFile.Handle, FileAccess.ReadWrite);
            currentSector = 0;
            numberOfSectors = 0;
            bytesPerSector = (int)drive.BytesPerSector;
        }

        public void Seek(long sectorNo)
        {
            innerStream.Seek(sectorNo * bytesPerSector, SeekOrigin.Begin);
            currentSector = sectorNo;
        }

        public void ReadSector(byte[] data)
        {
            innerStream.Read(data, 0, bytesPerSector);
            currentSector++;
        }

        public void WriteSector(byte[] data)
        {
            innerStream.Write(data, 0, bytesPerSector);
            currentSector++;
        }

        public void Flush()
        {
            innerStream.Flush();
        }
        public void Close()
        {
            Kernel32.CloseHandle(unmanagedFile.Handle);
        }

    }

}