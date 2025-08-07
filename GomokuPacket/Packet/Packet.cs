using MessagePack;

namespace GomokuPacket;

[MessagePackObject]
[Union(0, typeof(EnterPacket))]
public abstract class Packet
{
    [Key(0)]
    public PacketId PacketId { get; protected  set; }

    public Packet(PacketId packetId)
    {
        PacketId = packetId;
    }
}