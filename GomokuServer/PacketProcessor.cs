using System.Threading.Tasks.Dataflow;

public class PacketProcessor
{
    bool IsThreadRunning = false;

    private BufferBlock<PktBinaryRequestInfo> MsgBuffer = new();

    public void CreateAndStart()
    {
        new Thread(this.Process).Start();
    }

    void Process()
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