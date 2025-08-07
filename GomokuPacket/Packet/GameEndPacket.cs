using MessagePack;

namespace GomokuPacket;

[MessagePackObject]
public partial class GameEndPacket : Packet
{
    [Key(1)]
    public bool AmIWin { get; set; }

    public GameEndPacket() : base(PacketId.GameEnd) { }
}