namespace Log_temp_verloop_warmwelkom
{
    public class LOG_DoorOpened : LogItemDoor
    {
        public static bool VerifyType(LogItemDoor logItem)
        {
            if (logItem.Code == 0x6A)
            {
                return true;
            }
            return false;
        }
    }
}
