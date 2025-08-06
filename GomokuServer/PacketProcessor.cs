using System.Threading.Tasks.Dataflow;
using GomokuPacket;

public partial class PacketProcessor
{
    bool IsThreadRunning = false;

    private Dictionary<PacketId, Action<PktBinaryRequestInfo>> PacketHandlerMap;

    private BufferBlock<PktBinaryRequestInfo> MsgBuffer = new();

    public PacketProcessor()
    { 
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
                    handler(packet);
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

                continue;
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