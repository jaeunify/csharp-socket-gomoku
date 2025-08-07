using System.Threading.Tasks.Dataflow;
using GomokuPacket;
using MessagePack;

public partial class PacketProcessor
{
    bool IsThreadRunning = false;

    private Dictionary<PacketId, Action<string, Packet>> PacketHandlerMap;

    private BufferBlock<PktBinaryRequestInfo> MsgBuffer = new();
    private IPktBinarySender BinarySender;

    public PacketProcessor(IPktBinarySender packetSender)
    {
        // MainServer의 바이너리 Sender 인터페이스 등록
        BinarySender = packetSender;

        // 패킷 핸들러 등록
        PacketHandlerMap = new Dictionary<PacketId, Action<string, Packet>>
        {
            { PacketId.Enter, EnterProcess },
            { PacketId.SetRock, SetRockProcess },
        };
    }

    public void CreateAndStart()
    {
        IsThreadRunning = true;
        new Thread(this.Process).Start();
    }

    private void Process()
    {
        while (IsThreadRunning)
        {
            var serializedPacket = MsgBuffer.Receive();
            var senderSessionId = serializedPacket.SessionId;

            try
            {
                if (PacketHandlerMap.TryGetValue(serializedPacket.PacketID, out var handle) == false)
                    continue;

                var packet = MessagePackSerializer.Deserialize<Packet>(serializedPacket.Body);
                handle(senderSessionId, packet);
            }
            catch (ServerException ex)
            {
                SendPacket(senderSessionId, new ErrorPacket() { ErrorCode = ex.ErrorCode });
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

    public void Destroy()
    {
        IsThreadRunning = false;
    }

    public void InsertPacket(PktBinaryRequestInfo data)
    {
        MsgBuffer.Post(data);
    }

    public void SendPacket(string sessionId, Packet packet)
    {
        var body = MessagePackSerializer.Serialize(packet);
        var totalSize = (Int16)(body.Length + DIContainer.Get<ServerOption>().HeaderSize);

        List<byte> dataSource = new List<byte>();
        dataSource.AddRange(BitConverter.GetBytes(totalSize));
        dataSource.AddRange(BitConverter.GetBytes((Int16)packet.PacketId));
        dataSource.AddRange(body);

        BinarySender.Send(sessionId, dataSource.ToArray());
    }
}