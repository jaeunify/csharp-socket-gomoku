using System.Threading.Tasks.Dataflow;
using GomokuServer.Network.Handler;
using GomokuPacket;
using MessagePack;

namespace GomokuServer.Network;

public class PacketRouter
{
    private bool IsThreadRunning = false;
    private BufferBlock<PktBinaryRequestInfo> _MsgBuffer = new();
    private Action<string, byte[]> _SendBinary;

    public PacketRouter(Action<string, byte[]> sendBinaryAction)
    {
        _SendBinary = sendBinaryAction;
    }

    public void CreateAndStart()
    {
        IsThreadRunning = true;
        new Thread(this.Route).Start();
    }

    public void Destroy()
    {
        IsThreadRunning = false;
    }

    public void InsertPacket(PktBinaryRequestInfo data)
    {
        _MsgBuffer.Post(data);
    }

    public void SendPacket(string sessionId, Packet packet)
    {
        var body = MessagePackSerializer.Serialize(packet);
        var totalSize = (Int16)(body.Length + ServerOption.HeaderSize);

        List<byte> dataSource = new List<byte>();
        dataSource.AddRange(BitConverter.GetBytes(totalSize));
        dataSource.AddRange(body);

        _SendBinary(sessionId, dataSource.ToArray());
    }

    private void Route()
    {
        while (IsThreadRunning)
        {
            var serializedPacket = _MsgBuffer.Receive();
            var senderSessionId = serializedPacket.SessionId;

            try
            {
                var packet = MessagePackSerializer.Deserialize<Packet>(serializedPacket.Body);

                if (packet is EnterPacket enterPacket)
                {
                    new EnterPacketHandler(SendPacket).Handle(senderSessionId, enterPacket);
                }
                else if (packet is SetRockPacket setRockPacket)
                {
                    new SetRockHandler(SendPacket).Handle(senderSessionId, setRockPacket);
                }
                else
                {
                    throw new NotSupportedException($"Unsupported packet type: {packet.GetType()}");
                }
            }
            catch (Exception ex)
            {
                if (IsThreadRunning)
                {
                    // TODO log
                }

                throw; // impossible fatal error
            }
        }
    }
}