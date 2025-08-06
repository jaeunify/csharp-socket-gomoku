using MessagePack;

namespace GomokuPacket;

[MessagePackObject]
public partial class GameStartPacket : Packet
{
    [Key(1)]
    public bool AmIFirst { get; set; }

    public GameStartPacket() : base(PacketId.GameStart) { }
}