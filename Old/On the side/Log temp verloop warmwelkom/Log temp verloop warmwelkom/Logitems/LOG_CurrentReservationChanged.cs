namespace Log_temp_verloop_warmwelkom
{
    public class LOG_CurrentReservationChanged : LogItemGateway
    {
        public float Setpoint
        {
            get
            {
                return System.BitConverter.ToSingle(Data, 3);
            }
        }

        public override bool TryPopulate(string data)
        {
            if (!TryPopulateBase(data))
                return false;

            if (Code == 0xA0)
            {
                if (Data.CompareBegin(new byte[] { 0x00, 0x0B }))
                    return true;
            }
            return false;
        }
    }
}
