using System;
using System.Text;

namespace STDLib.JBVProtocol
{

    public class Frame
    {
        public Options Opts { get; set; } = 0;
        public byte Hops { get; set; } = 0;
        public UInt32 TxID { get; set; } = 0;
        public UInt32 RxID { get; set; } = 0;
        public UInt16 Sequence { get; set; } = 0;
        UInt16 DataLength { get; set; } = 0;
        byte[] Data { get; set; }



        public int GetTotalLength()
        {
            return 14 + DataLength;
        }

        [Flags]
        public enum Options
        {
            None = 0,
            Broadcast = (1 << 0),   // when true, message is send to all clients in network. When frame is reply and broadcast, the frame will not be handled as broadcast!
            ASCII =     (1 << 1),   // when true, commands are send as ASCII otherwise commands are send as raw data, currently only ascii mode supported.
            Request =   (1 << 2),   // frame is either a request or a reply.
            //RFU		= (1<<3),	// Suggestion: CRC, true when CRC of frame is also send. Might be helpful when using things like RS232.
            //RFU 		= (1<<4),	// Suggestion: Encryption, Together with next field determines what encryption is used. (Since this is the first byte, the rest of the message can be encrypted.)
            //RFU 		= (1<<5),	// Suggestion: Encryption,
            //RFU		= (1<<6),	//
            //RFU		= (1<<7),	//
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
                    case 0: return (byte)Opts;
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
                    case 0: Opts = (Options)value; break;
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