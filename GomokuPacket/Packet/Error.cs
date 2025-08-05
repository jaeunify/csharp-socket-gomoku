using MessagePack;

namespace GomokuPacket;

[MessagePackObject]
public class Error : Packet
{
    [Key(1)]
    public ERROR_CODE ErrorCode { get; set; }
}