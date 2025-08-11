using GomokuPacket;

namespace GomokuServer.Network.Handler;

public abstract class PacketHandler<TPacket> where TPacket : Packet
{
    protected Action<string, Packet> SendPacket { get; private set; }
    public PacketHandler(Action<string, Packet> sendPacket)
    {
        SendPacket = sendPacket;
    }
    public abstract void Handle(string sessionId, TPacket packet);
}