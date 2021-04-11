using System;
using System.Text;

namespace STDLib.JBVProtocol
{

    public class Frame
    {
        byte Opt { get; set; } = 0;
        public byte Hops { get; set; } = 0;
        public UInt32 TxID { get; set; } = 0;
        public UInt32 RxID { get; set; } = 0;
        public UInt16 Sequence { get; set; } = 0;
        UInt16 DataLength { get; set; } = 0;
        byte[] Data { get; set; }



        public FrameTypes Type
        {
            get { return (FrameTypes)((Opt & 0xF0) >> 4); }
            set { Opt = (byte)((Opt & 0x0F) | (((byte)value << 4) & 0xF0)); }
        }

        public FrameOptions Options
        {
            get { return (FrameOptions)((Opt & 0x0F)); }
            set { Opt = (byte)((Opt & 0xF0) | ((byte)value & 0x0F)); }
        }


        public int GetTotalLength()
        {
            return 14 + DataLength;
        }

        [Flags]
        public enum FrameOptions
        {
            None = 0,
            Broadcast = (1 << 0),   //Frame will be send to all within the network.
            ASCII = (1 << 1),	    //The data field has to be interpreted as ASCII, also the reply will be send in ASCII.
        }

        public enum FrameTypes
        {
            ProtocolFrame = 0x00,
            DataFrame = 0x01,
        }


        int ReplaceByte(int index, int value, byte replaceByte)
        {
            return (value & ~(0xFF << index)) | (replaceByte << index);
        }

        public void SetData(byte[] data)
        {
            Data = data;
            DataLength = (UInt16)data.Length;
        }

        public byte[] GetData()
        {
            return Data;
        }

        public override string ToString()
        {
            return $"({TxID} -> {RxID})";
        }

        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return (byte)Opt;
                    case 1: return (byte)Hops;
                    case 2: return (byte)(TxID >> 24);
                    case 3: return (byte)(TxID >> 16);
                    case 4: return (byte)(TxID >> 8);
                    case 5: return (byte)TxID;
                    case 6: return (byte)(RxID >> 24);
                    case 7: return (byte)(RxID >> 16);
                    case 8: return (byte)(RxID >> 8);
                    case 9: return (byte)RxID;
                    case 10: return (byte)(Sequence >> 8);
                    case 11: return (byte)Sequence;
                    case 12: return (byte)(DataLength >> 8);
                    case 13: return (byte)DataLength;
                    default: return Data[index - 14];
                }
            }
            set
            {
                switch (index)
                {
                    case 0: Opt = value; break;
                    case 1: Hops = value; break;
                    case 2: TxID = (UInt32)ReplaceByte(24, (int)TxID, value); break;
                    case 3: TxID = (UInt32)ReplaceByte(16, (int)TxID, value); break;
                    case 4: TxID = (UInt32)ReplaceByte(08, (int)TxID, value); break;
                    case 5: TxID = (UInt32)ReplaceByte(00, (int)TxID, value); break;
                    case 6: RxID = (UInt32)ReplaceByte(24, (int)RxID, value); break;
                    case 7: RxID = (UInt32)ReplaceByte(16, (int)RxID, value); break;
                    case 8: RxID = (UInt32)ReplaceByte(08, (int)RxID, value); break;
                    case 9: RxID = (UInt32)ReplaceByte(00, (int)RxID, value); break;
                    case 10: Sequence = (UInt16)ReplaceByte(8, Sequence, value); break;
                    case 11: Sequence = (UInt16)ReplaceByte(0, Sequence, value); break;
                    case 12: DataLength = (UInt16)ReplaceByte(8, DataLength, value); break;
                    case 13: DataLength = (UInt16)ReplaceByte(0, DataLength, value); break;
                    default:
                        if (Data == null)
                            Data = new byte[DataLength];
                        Data[index - 14] = value;
                        break;
                }
            }
        }
    }

}