using GomokuPacket;

public interface Handler
{
    public abstract static void RegistPacketHandler(Dictionary<PacketId, Action<PktBinaryRequestInfo>> packetHandlerMap);
}