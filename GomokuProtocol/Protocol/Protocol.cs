using MessagePack;

namespace GomokuProtocol;

[MessagePackObject]
public class Protocol
{
    [Key(0)]
    public PacketId PacketId { get; set; }
}