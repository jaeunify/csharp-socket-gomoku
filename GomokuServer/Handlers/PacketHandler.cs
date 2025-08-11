using GomokuPacket;
using GomokuServer.Users;
using GomokuServer.Rooms;

namespace GomokuServer.Handlers;

public abstract class PacketHandler<TPacket> where TPacket : Packet
{
    protected Action<string, Packet> SendPacket { get; private set; }
    protected UserManager? UserManager { get; private set; }
    protected RoomManager? RoomManager { get; private set; }
    public PacketHandler(Action<string, Packet> sendPacket, UserManager userManager = null, RoomManager roomManager = null)
    {
        SendPacket = sendPacket;
        UserManager = userManager;
        RoomManager = roomManager;
    }
    public abstract void Handle(string sessionId, TPacket packet);
}