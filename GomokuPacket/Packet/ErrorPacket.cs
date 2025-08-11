using MessagePack;

namespace GomokuPacket;

[MessagePackObject]
public partial class ErrorPacket : Packet
{
    [Key(1)]
    public ErrorCode ErrorCode { get; set; }

    public ErrorPacket() : base(PacketId.Error) { }
}