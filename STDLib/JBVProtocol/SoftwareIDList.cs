using System;


namespace STDLib.JBVProtocol
{
    public enum SoftwareID : UInt32
    {
        Unknown = 0,
        //Router = 1,
        LeaseServer = 2,
        TileMapClient = 3,
        TileMapServer = 4,
        DPS50xx = 5,
        TestApp = 6,    //This id can be used when testing stuff. Its a device that doens't really exists.
        ConnectionServer = 7,
        DebugTool = 8,
        FunctionGenerator = 9,
        LogHandler = 10,
    }
}