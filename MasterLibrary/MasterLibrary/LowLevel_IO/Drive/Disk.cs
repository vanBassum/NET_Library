using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Text;
using MasterLibrary.LowLevel_IO.PInvoke;

namespace MasterLibrary.LowLevel_IO.Drive
{
    public class Disk : Stream
    {
        Stream innerStream;
        UnmanagedFileLoader unmanagedFile;


        public Disk()
        {

        }
        public void OpenVolume(string driveLetter)
        {
            unmanagedFile = new UnmanagedFileLoader(string.Format("\\\\.\\{0}:", driveLetter));
            innerStream = new FileStream(unmanagedFile.Handle, FileAccess.ReadWrite);
        }


        public void OpenDrive(int driveNumber)
        {
            unmanagedFile = new UnmanagedFileLoader(string.Format("\\\\.\\PhysicalDrive{0}", driveNumber));
            innerStream = new FileStream(unmanagedFile.Handle, FileAccess.ReadWrite);
        }

        public void Open(WIN32_DiskDrive drive)
        {
            unmanagedFile = new UnmanagedFileLoader(drive.Name);
            innerStream = new FileStream(unmanagedFile.Handle, FileAccess.ReadWrite);
        }

        public override void Close()
        {
            Kernel32.CloseHandle(unmanagedFile.Handle);
        }


        public override bool CanRead => innerStream.CanRead;

        public override bool CanSeek => innerStream.CanSeek;

        public override bool CanWrite => innerStream.CanWrite;

        public override long Length => innerStream.Length;

        public override long Position { get => innerStream.Position; set => innerStream.Position = value; }

        public override void Flush()
        {
            innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return innerStream.Read(buffer, offset, count);
        }

        public int Read(byte[] buffer)
        {
            return Read(buffer, 0, buffer.Length);
        }

        public byte[] Read(int length)
        {
            byte[] data = new byte[length];
            Read(data, 0, data.Length);
            return data;
        }


        public override long Seek(long offset, SeekOrigin origin)
        {
            return innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            innerStream.Write(buffer, offset, count);
        }

        public void Write(byte[] buffer)
        {
            innerStream.Write(buffer, 0, buffer.Length);
        }
    }

}