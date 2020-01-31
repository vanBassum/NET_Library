namespace Log_temp_verloop_warmwelkom
{
    public class LOG_Restarted : LogItemGateway
    {

        public override bool TryPopulate(string data)
        {
            if (!TryPopulateBase(data))
                return false;

            if (Code == 0x02)
                return true;
            return false;
        }
    }
}
