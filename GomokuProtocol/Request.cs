using MessagePack;

namespace GomokuProtocol;

[MessagePackObject]
public class Request
{
    [Key(0)]
    public PacketId Id { get; set; }

    public Request(PacketId id)
    {
        Id = id;
    }
}