using GomokuPacket;
using MessagePack;

[MessagePackObject]
public partial class EnterPacket : Packet
{
    [Key(1)]
    public required string SenderSessionId { get; set; }

    public EnterPacket() : base(PacketId.Enter) { }
}