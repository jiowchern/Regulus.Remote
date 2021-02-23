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
        AddPropertySoul,
        RemovePropertySoul,

    }
}
