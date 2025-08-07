using GomokuPacket;
using MessagePack;

[MessagePackObject]
public partial class SetRockPacket : Packet
{
    [Key(1)]
    public int X { get; set; }

    [Key(2)]
    public int Y { get; set; }

    public SetRockPacket() : base(PacketId.SetRock) { }
}