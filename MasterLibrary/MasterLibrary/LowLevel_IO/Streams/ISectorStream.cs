using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.LowLevel_IO.Streams
{
    public interface ISectorStream
    {
        
        void ReadSector(byte[] data);   //Read 1 sector
        void WriteSector(byte[] data);  //Write 1 sector
        void Flush();                   //Flush the stream
        void Seek(Int64 sectorNo);     //Goto sector x
        Int64 CurrentSector { get; }   //Stream position in sectors
        Int64 NumberOfSectors { get; } //Stream length in sectors
        Int32 BytesPerSector { get; }  //Number of bytes in a sector
        //void Open();                    //Open the stream
        void Close();                   //Close the stream

        bool CanRead  { get;}           
        bool CanSeek  { get;} 
        bool CanWrite { get; }

    }
}
