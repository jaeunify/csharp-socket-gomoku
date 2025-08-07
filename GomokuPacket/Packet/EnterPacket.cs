using GomokuPacket;
using MessagePack;

[MessagePackObject]
public partial class EnterPacket : Packet
{
    public EnterPacket() : base(PacketId.Enter) { }
}