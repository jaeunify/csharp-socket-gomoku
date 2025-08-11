using GomokuPacket;

namespace Gomoku.Network.Handler;

public abstract class PacketHandler<TPacket> where TPacket : Packet
{
    public Action<string, Packet> SendPacket;
    public PacketHandler(Action<string, Packet> sendPacket)
    {
        SendPacket = sendPacket;
    }
    public abstract void Handle(string sessionId, TPacket packet);
}