namespace Log_temp_verloop_warmwelkom
{

    public class LOG_Relais2 : LogItemGateway
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
                if (Data[1] == 0x0A)
                    return true;
            }

            return false;
        }
    }
}
