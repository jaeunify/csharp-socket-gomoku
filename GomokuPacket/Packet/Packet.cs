using MessagePack;

namespace GomokuPacket;

[MessagePackObject]
[Union((short)PacketId.Enter, typeof(EnterPacket))]
[Union((short)PacketId.GameStart, typeof(GameStartPacket))]
[Union((short)PacketId.SetRock, typeof(SetRockPacket))]
[Union((short)PacketId.GameEnd, typeof(GameEndPacket))]
public abstract class Packet
{
    [Key(0)]
    public PacketId PacketId { get; protected  set; }

    public Packet(PacketId packetId)
    {
        PacketId = packetId;
    }
}