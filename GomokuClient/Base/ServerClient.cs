using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;
using GomokuPacket;
using MessagePack;
using System.Text.Json;

public class ServerClient : Instance
{
    const int HEADER_SIZE = 2;

    public string Ip { get; set; }
    public int Port { get; set; }
    public LogState LogState { get; set; }
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isRunning = false;
    public event Action<Packet>? OnReceivePacket;
    public event Action<string>? OnReceiveErrorPacket;
    public event Action<Packet>? OnSendPacket;
    public event Action? OnError;

    public ServerClient(LogState logstore)
    {
        this.LogState = logstore;
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


    /// <returns>int byteCount: 전송한 바이너리의 전체 길이</returns>
    public async Task<int> SendAsync(Packet packet, bool triggerEvent = false)
    {
        if (stream == null)
            throw new Exception("서버 연결이 되어 있지 않습니다");

        var body = MessagePackSerializer.Serialize(packet);
        short totalSize = (short)(HEADER_SIZE + body.Length);

        var bytes = new List<byte>();
        bytes.AddRange(BitConverter.GetBytes(totalSize));
        bytes.AddRange(body);

        await stream.WriteAsync(bytes.ToArray(), 0, bytes.Count);

        if (triggerEvent)
        {
            OnSendPacket?.Invoke(packet);
        }

        LogState.AddLog($"[전송] {packet.PacketId} {JsonSerializer.Serialize(packet)}");
        return bytes.Count;
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

                var packets = ParsePackets(buffer, bytesRead);
                foreach (var packet in packets)
                {
                    ProcessPacket(packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReceiveLoop Error] {ex.Message}");
                Disconnect();
                break;
            }
        }
    }

    private List<Packet> ParsePackets(byte[] data, int length)
    {
        int offset = 0;
        var packets = new List<Packet>();

        while (length - offset >= HEADER_SIZE)
        {
            short totalSize = BitConverter.ToInt16(data, offset);

            if (length - offset < totalSize)
                break; // 데이터 부족

            byte[] body = new byte[totalSize - HEADER_SIZE];
            Buffer.BlockCopy(data, offset + HEADER_SIZE, body, 0, body.Length);

            var packet = MessagePackSerializer.Deserialize<Packet>(body);
            packets.Add(packet);
            LogState?.AddLog($"[응답] {packet.PacketId} {JsonSerializer.Serialize(packet, packet.GetType())}");
            offset += totalSize;
        }

        return packets;
    }

    private void ProcessPacket(Packet packet)
    {
        if (packet.PacketId > 0)
        {
            OnReceivePacket?.Invoke(packet);
        }
        else if (packet is ErrorPacket errorPacket)
        {
            OnError?.Invoke();

            var errorMessage = errorPacket.ErrorCode switch
            {
                ERROR_CODE.USER_COUNT_FULL => "서버에 접속 가능한 유저 수가 가득 찼습니다. 접속 중인 클라이언트를 종료하고 다시 시도해주세요.",
                ERROR_CODE.USER_ALREADY_EXIST => "이미 동일한 세션 아이디로 Enter 하셨습니다. 다음 프로토콜인 SetRock을 진행하세요.",
                ERROR_CODE.UNKNOWN_USER => "존재하지 않는 유저입니다.",
                ERROR_CODE.UNENTERED_USER => "Enter 프로토콜을 먼저 진행해야 합니다.",
                ERROR_CODE.GAME_UNSTARTED => "게임이 시작되지 않은 상태입니다. 상대방이 입장하기를 기다리세요.",
                ERROR_CODE.INVALID_ROCK_POSITION => "x,y는 0이상 14이하여야 합니다. 다시 놓아 주세요.",
                ERROR_CODE.ALREADY_SET_ROCK_POSITION => "이미 그 자리는 다른 돌이 놓여 있습니다. 다시 놓아 주세요.",
                ERROR_CODE.NOT_MY_TURN => "당신의 차례가 아닙니다. 상대방의 차례를 기다리세요.",
                _ => throw new Exception("알 수 없는 에러 코드")
            };
            OnReceiveErrorPacket?.Invoke(errorMessage);
        }
    }
}
