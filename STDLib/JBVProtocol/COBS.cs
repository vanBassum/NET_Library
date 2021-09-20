namespace STDLib.JBVProtocol
{
    public static class COBS
    {
        static int DIVUP(int n, int d)
        {
            return (n - 1) / d + 1;
        }

        public static int CalcEncodedSize(int e)
        {
            return DIVUP(e, 254) + e + 1;
        }

        static int CalcDecodedSize(int e)
        {
            return (e - 1) - DIVUP(e - 1, 255);
        }

        public static byte[] Encode(byte[] src)
        {
            int eSize = CalcEncodedSize(src.Length);
            int cInd = 0;
            int srcInd = 0;
            int dstInd = 1;
            byte code = 0x01;
            byte[] dst = new byte[eSize];

            while(srcInd < src.Length)
            {
                if (src[srcInd] == 0)
                {
                    dst[cInd] = code;
                    cInd = dstInd++;
                    code = 1;
                }
                else
                {
                    dst[dstInd++] = src[srcInd];
                    code++;
                    if(code == 0xFF)
                    {
                        dst[cInd] = code;
                        cInd = dstInd++;
                        code = 1;
                    }
                    
                }
                srcInd++;
            }
            dst[cInd] = code;
            cInd = dstInd++;
            code = 1;
            dst[cInd] = 0;
            return dst;
        }

        public static byte[] Decode(byte[] src)
        {
            //int dSize = CalcDecodedSize(src.Length);
            byte[] dst = new byte[src.Length];
            int srcInd = 0;
            int dstInd = 0;
            while (srcInd < src.Length)
            {
                int i, code = src[srcInd++];
                for (i = 1; i < code; i++)
                    dst[dstInd++] = src[srcInd++];
                if (code < 0xFF)
                    dst[dstInd++] = 0;
            }
            //@TODO Fix length
            return dst;
        }
    }
}
