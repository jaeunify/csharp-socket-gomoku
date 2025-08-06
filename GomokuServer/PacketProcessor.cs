using System.Threading.Tasks.Dataflow;
using GomokuPacket;

public partial class PacketProcessor
{
    bool IsThreadRunning = false;

    private Dictionary<PacketId, Action<PktBinaryRequestInfo>> PacketHandlerMap;

    private BufferBlock<PktBinaryRequestInfo> MsgBuffer = new();
    private IPktBinarySender PacketSender;

    public PacketProcessor(IPktBinarySender packetSender)
    { 
        // MainServer의 바이너리 Sender 인터페이스 등록
        PacketSender = packetSender;

        // 패킷 핸들러 등록
        PacketHandlerMap = new Dictionary<PacketId, Action<PktBinaryRequestInfo>>
        {
            { PacketId.Connect, ConnectProcess },
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
            try
            {
                var packet = MsgBuffer.Receive();

                // TODO log

                if (PacketHandlerMap.TryGetValue(packet.PacketID, out var handler))
                    handler(packet); // todo PktBinaryRequestInfo 에서 packet으로 deseriazlize해서 handler 호출
            }
            catch (ServerException ex)
            {
                // TODO error packet send

                continue;
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
}