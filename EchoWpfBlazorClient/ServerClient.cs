using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;

public class ServerClient : Instance
{
    const int HEADER_SIZE = 4;

    public string Ip { get; set; }
    public int Port { get; set; }
    public LogStore LogStore { get; set; }

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isRunning = false;

    public ConcurrentQueue<(short PacketId, byte[] Body)> RecvPacketQueue = new();

    public ServerClient(LogStore logstore)
    {
        this.LogStore = logstore;
    }

    public void Configure(string address, int port)
    {
        Ip = address;
        Port = port;
    }

    public async Task ConnectAsync()
    {
        client = new TcpClient();
        await client.ConnectAsync(Ip, Port);
        stream = client.GetStream();

        isRunning = true;
        receiveThread = new Thread(ReceiveLoop);
        receiveThread.Start();
    }

    public void Disconnect()
    {
        isRunning = false;
        stream?.Close();
        client?.Close();
    }

    public async Task<int> SendAsync(string sendText, short packetId = 1000)
    {
        if (stream == null)
            throw new Exception("서버 연결이 되어 있지 않습니다");

        var body = Encoding.UTF8.GetBytes(sendText);
        short totalSize = (short)(HEADER_SIZE + body.Length);

        var packet = new List<byte>();
        packet.AddRange(BitConverter.GetBytes(totalSize));
        packet.AddRange(BitConverter.GetBytes(packetId));
        packet.AddRange(body);

        await stream.WriteAsync(packet.ToArray(), 0, packet.Count);
        return packet.Count;
    }

    private void ReceiveLoop()
    {
        var buffer = new byte[8192];

        while (isRunning)
        {
            try
            {
                if (!client.Connected || stream == null) break;

                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    Disconnect();
                    break;
                }

                ParsePackets(buffer, bytesRead);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReceiveLoop Error] {ex.Message}");
                Disconnect();
                break;
            }
        }
    }

    private void ParsePackets(byte[] data, int length)
    {
        int offset = 0;

        while (length - offset >= HEADER_SIZE)
        {
            short totalSize = BitConverter.ToInt16(data, offset);
            short packetId = BitConverter.ToInt16(data, offset + 2);

            if (length - offset < totalSize)
                break; // 데이터 부족

            byte[] body = new byte[totalSize - HEADER_SIZE];
            Buffer.BlockCopy(data, offset + HEADER_SIZE, body, 0, body.Length);

            string msg = Encoding.UTF8.GetString(body);
            LogStore?.AddLog($"[서버 응답] PacketID: {packetId}, Message: {msg}");

            RecvPacketQueue.Enqueue((packetId, body));
            offset += totalSize;
        }
    }
}
