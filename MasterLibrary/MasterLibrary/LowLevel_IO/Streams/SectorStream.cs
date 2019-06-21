using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.LowLevel_IO.Streams
{
    public class SectorStream : Stream
    {
        ISectorStream sectorStream;

        byte[] innerBuffer;                                                         //The innerBuffer always contains the sector in wich position is located

        Int64 position = 0;

        private Int64 Sector { get => position / sectorStream.BytesPerSector; }
        private Int32 PosInSect { get => (Int32)(position % sectorStream.BytesPerSector); }


        public SectorStream(ISectorStream stream)
        {
            sectorStream = stream;
            sectorStream.Seek(0);                                                   //Set stream to beginning
            innerBuffer = new byte[sectorStream.BytesPerSector];                    //Create the buffer
            sectorStream.ReadSector(innerBuffer);                                   //Load sector 0 into buffer
            position = 0;                                                           
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if(origin != SeekOrigin.Begin)
                throw new NotImplementedException();

            position = offset;
            sectorStream.Seek(Sector);
            sectorStream.ReadSector(innerBuffer);                                   //Load sector into buffer
            return position;
        }


        public override int Read(byte[] buffer, int offset, int count)
        {
            
            if(offset != 0)
                throw new NotImplementedException();

            Int64 dest = 0;

            //Copy all complete sectors nessesairy
            while (count > 0)
            {
                int length = Math.Min(sectorStream.BytesPerSector - PosInSect, count);      //Number of bytes to copy
                Array.Copy(innerBuffer, PosInSect, buffer, dest, length);                   //Copy the bytes
                position += length;                                                         //Increase counters
                dest += length;
                count -= length;

                if(Sector == sectorStream.CurrentSector)                                    //Read next sector if nessesairy
                    sectorStream.ReadSector(innerBuffer);

                if (Sector != sectorStream.CurrentSector - 1)                               //If something went wrong, make sure the right sector is in the buffer                           
                {
                    sectorStream.Seek(Sector);
                    sectorStream.ReadSector(innerBuffer);
                }
            }

            return (int)dest;
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





        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }


        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        



        public override bool CanRead => sectorStream.CanRead;

        public override bool CanSeek => sectorStream.CanSeek;

        public override bool CanWrite => sectorStream.CanWrite;

        public override long Length => (sectorStream.BytesPerSector * sectorStream.NumberOfSectors);

        public override long Position { get => position; set => throw new NotImplementedException(); }


    }
}
