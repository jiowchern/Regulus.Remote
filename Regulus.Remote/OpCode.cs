namespace Regulus.Remote
{
    public enum ClientToServerOpCode : byte
    {
        CallMethod = 1,

        Ping,

        Release,

        UpdateProperty,
        AddEvent,
        RemoveEvent,
        AddNotifierSupply,
        RemoveNotifierSupply,
        AddNotifierUnsupply,
        RemoveNotifierUnsupply,
    };

    public enum ServerToClientOpCode : byte
    {
        InvokeEvent = 1,

        LoadSoul,

        UnloadSoul,

        ReturnValue,


        LoadSoulCompile,

        Ping,

        ErrorMethod,

        ProtocolSubmit,
        SetProperty,
    }
}
