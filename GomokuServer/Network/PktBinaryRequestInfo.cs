using GomokuPacket;
using SuperSocketLite.SocketBase.Protocol;

namespace Gomoku.Network;

public class PktBinaryRequestInfo : BinaryRequestInfo
{
    // in header
    public UInt16 TotalSize { get; private set; }

    // out of header
    public string SessionId { get; set; }

    public PktBinaryRequestInfo(UInt16 totalSize, byte[] body)
        : base(null, body)
    {
        this.TotalSize = totalSize;
    }
}