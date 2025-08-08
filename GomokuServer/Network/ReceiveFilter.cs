using SuperSocketLite.Common;
using SuperSocketLite.SocketEngine.Protocol;

public class ReceiveFilter : FixedHeaderReceiveFilter<PktBinaryRequestInfo>
{
    public ReceiveFilter() : base(ServerOption.HeaderSize)
    {
    }

    protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
    {
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(header, offset, 2);
        }

        var packetTotalSize = BitConverter.ToUInt16(header, offset);
        return packetTotalSize - ServerOption.HeaderSize;
    }

    protected override PktBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
    {
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(header.Array, 0, ServerOption.HeaderSize);
        }

        return new PktBinaryRequestInfo(BitConverter.ToUInt16(header.Array, 0),
                                       BitConverter.ToUInt16(header.Array, 0 + 2),
                                       bodyBuffer.CloneRange(offset, length));
    }
}

