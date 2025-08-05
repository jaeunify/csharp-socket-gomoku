using MessagePack;

[MessagePackObject]
public class ConnectRequest : Request
{
    public ConnectRequest() : base(PacketId.Connect)
    {
    }
}

[MessagePackObject]
public class ConnectResponse : Response
{
    [Key(2)]
    public int PlayerId { get; set; }
    
    public ConnectResponse() : base(PacketId.Connect)
    {
    }
}