using SuperSocketLite.Common;
using SuperSocketLite.SocketEngine.Protocol;

namespace GomokuServer.Network;

public class ReceiveFilter : FixedHeaderReceiveFilter<PktBinaryRequestInfo>
{
    public ReceiveFilter() : base(ServerConfig.HeaderSize) { }

    protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
    {
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(header, offset, 2);
        }

        var packetTotalSize = BitConverter.ToUInt16(header, offset);
        return packetTotalSize - ServerConfig.HeaderSize;
    }

    protected override PktBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
    {
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(header.Array, 0, ServerConfig.HeaderSize);
        }

        return new PktBinaryRequestInfo(BitConverter.ToUInt16(header.Array, 0), bodyBuffer.CloneRange(offset, length));
    }
}

