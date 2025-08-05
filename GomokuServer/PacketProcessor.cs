using System.Threading.Tasks.Dataflow;
using GomokuPacket;

public class PacketProcessor
{
    bool IsThreadRunning = false;

    private Dictionary<PacketId, Action<PktBinaryRequestInfo>> PacketHandlerMap = new();

    private BufferBlock<PktBinaryRequestInfo> MsgBuffer = new();

    public void CreateAndStart()
    {
        // Regist Packet Handler
        CommonHandler.RegistPacketHandler(PacketHandlerMap);
        RoomHandler.RegistPacketHandler(PacketHandlerMap);

        new Thread(this.Process).Start();
    }

    private void Process()
    {
        while (IsThreadRunning)
        {
            try
            {
                var packet = MsgBuffer.Receive();

                // TODO 
            }
            catch (Exception ex)
            {
                if (IsThreadRunning)
                {
                    // TODO log
                }
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