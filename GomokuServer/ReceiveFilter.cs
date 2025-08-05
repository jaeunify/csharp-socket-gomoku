using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GomokuProtocol;

using SuperSocketLite.Common;
using SuperSocketLite.SocketBase.Protocol;
using SuperSocketLite.SocketEngine.Protocol;

public class PktBinaryRequestInfo : BinaryRequestInfo
{
    // 패킷 헤더용 변수
    public UInt16 TotalSize { get; private set; }
    public PacketId PacketID { get; private set; }

    public const int HEADERE_SIZE = 4;


    public PktBinaryRequestInfo(UInt16 totalSize, ushort packetID, byte[] body)
        : base(null, body)
    {
        this.TotalSize = totalSize;
        this.PacketID = (PacketId)packetID;
    }
}

public class ReceiveFilter : FixedHeaderReceiveFilter<PktBinaryRequestInfo>
{
    public ReceiveFilter() : base(PktBinaryRequestInfo.HEADERE_SIZE)
    {
    }

    protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
    {
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(header, offset, 2);

        var packetTotalSize = BitConverter.ToUInt16(header, offset);
        return packetTotalSize - PktBinaryRequestInfo.HEADERE_SIZE;
    }

    protected override PktBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
    {
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(header.Array, 0, PktBinaryRequestInfo.HEADERE_SIZE);

        return new PktBinaryRequestInfo(BitConverter.ToUInt16(header.Array, 0),
                                       BitConverter.ToUInt16(header.Array, 0 + 2),
                                       bodyBuffer.CloneRange(offset, length));
    }
}

