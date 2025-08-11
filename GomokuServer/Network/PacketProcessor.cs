using System.Threading.Tasks.Dataflow;
using MessagePack;
using GomokuPacket;
using GomokuServer.Handlers;
using GomokuServer.Users;
using GomokuServer.Rooms;

namespace GomokuServer.Network;

public class PacketProcessor
{
    private bool _isThreadRunning = false;
    private BufferBlock<PktBinaryRequestInfo> _msgBuffer = new();
    private Action<string, byte[]> _sendBinary;
    private UserManager _userManager = new();
    private RoomManager _roomManager = new();
    private Dictionary<PacketId, Action<string, Packet>> _packetHandlers = new();

    public PacketProcessor(Action<string, byte[]> sendBinaryAction)
    {
        _sendBinary = sendBinaryAction;

        var enterHandler = new EnterPacketHandler(SendPacket, _userManager, _roomManager);
        _packetHandlers.Add(PacketId.Enter, (sessionId, packet) => enterHandler.Handle(sessionId, (EnterPacket)packet));
        var setRockHandler = new SetRockHandler(SendPacket, _userManager, _roomManager);
        _packetHandlers.Add(PacketId.SetRock, (sessionId, packet) => setRockHandler.Handle(sessionId, (SetRockPacket)packet));
    }

    public void CreateAndStart()
    {
        _isThreadRunning = true;
        new Thread(this.Process).Start();
    }

    public void Destroy()
    {
        _isThreadRunning = false;
    }

    public void InsertPacket(PktBinaryRequestInfo data)
    {
        _msgBuffer.Post(data);
    }

    public void SendPacket(string sessionId, Packet packet)
    {
        var body = MessagePackSerializer.Serialize(packet);
        var totalSize = (Int16)(body.Length + ServerConfig.HeaderSize);

        List<byte> dataSource = new List<byte>();
        dataSource.AddRange(BitConverter.GetBytes(totalSize));
        dataSource.AddRange(body);

        _sendBinary(sessionId, dataSource.ToArray());
    }

    private void Process()
    {
        while (_isThreadRunning)
        {
            var serializedPacket = _msgBuffer.Receive();
            var senderSessionId = serializedPacket.SessionId;

            try
            {
                var packet = MessagePackSerializer.Deserialize<Packet>(serializedPacket.Body);

                switch (packet)
                {
                    case EnterPacket enterPacket: new EnterPacketHandler(SendPacket, _userManager, _roomManager).Handle(senderSessionId, enterPacket); break;
                    case SetRockPacket setRockPacket: new SetRockHandler(SendPacket,  _userManager, _roomManager).Handle(senderSessionId, setRockPacket); break;
                    default: throw new NotSupportedException($"Unsupported packet type: {packet.GetType().Name}");
                }
            }
            catch (Exception ex)
            {
                if (_isThreadRunning)
                {
                    // TODO log
                }

                throw; // impossible fatal error
            }
        }
    }
}