namespace Log_temp_verloop_warmwelkom
{
    public class LOG_HeatingRequest : LogItemGateway
    {
        public bool Active
        {
            get
            {
                return Data[3] > 0;
            }
        }

        public override bool TryPopulate(string data)
        {
            if (!TryPopulateBase(data))
                return false;

            if (Code == 0xA0)
            {
                if (Data.CompareBegin(new byte[] { 0x01, 0x05 }))
                    return true;
            }
            return false;
        }
    }


}
