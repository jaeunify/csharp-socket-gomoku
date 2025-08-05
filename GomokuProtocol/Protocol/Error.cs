using MessagePack;

namespace GomokuProtocol;

[MessagePackObject]
public class Error : Protocol
{
    [Key(1)]
    public ERROR_CODE ErrorCode { get; set; }
}