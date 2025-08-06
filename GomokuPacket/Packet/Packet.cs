using MessagePack;

namespace GomokuPacket;

[MessagePackObject]
public class Packet
{
    [Key(0)]
    public PacketId PacketId { get; protected  set; }

    public Packet(PacketId packetId)
    {
        PacketId = packetId;
    }
}