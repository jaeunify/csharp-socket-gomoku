using MessagePack;

namespace GomokuProtocol;

[MessagePackObject]
public class Response
{
    [Key(0)]
    public PacketId Id { get; set; }
    [Key(1)]
    public bool Success { get; set; }

    public Response(PacketId id)
    {
        Id = id;
        Success = true;
    }
}