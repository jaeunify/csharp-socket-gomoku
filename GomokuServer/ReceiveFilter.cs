using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GomokuPacket;

using SuperSocketLite.Common;
using SuperSocketLite.SocketBase.Protocol;
using SuperSocketLite.SocketEngine.Protocol;

public class PktBinaryRequestInfo : BinaryRequestInfo
{
    // in header
    public UInt16 TotalSize { get; private set; }
    public PacketId PacketID { get; private set; }

    // out of header
    public string SessionId { get; set; }

    public PktBinaryRequestInfo(UInt16 totalSize, ushort packetID, byte[] body)
        : base(null, body)
    {
        this.TotalSize = totalSize;
        this.PacketID = (PacketId)packetID;
    }
}

public class ReceiveFilter : FixedHeaderReceiveFilter<PktBinaryRequestInfo>
{
    public ReceiveFilter() : base(DIContainer.Get<ServerOption>().HeaderSize)
    {
    }

    protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
    {
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(header, offset, 2);

        var packetTotalSize = BitConverter.ToUInt16(header, offset);
        return packetTotalSize - DIContainer.Get<ServerOption>().HeaderSize;
    }

    protected override PktBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
    {
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(header.Array, 0, DIContainer.Get<ServerOption>().HeaderSize);

        return new PktBinaryRequestInfo(BitConverter.ToUInt16(header.Array, 0),
                                       BitConverter.ToUInt16(header.Array, 0 + 2),
                                       bodyBuffer.CloneRange(offset, length));
    }
}

