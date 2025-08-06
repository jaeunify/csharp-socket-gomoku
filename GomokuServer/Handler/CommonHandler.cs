using GomokuPacket;

public class CommonHandler : Handler
{
    public static void RegistPacketHandler(Dictionary<PacketId, Action<PktBinaryRequestInfo>> packetHandlerMap)
    {
        packetHandlerMap.Add(PacketId.Connect, Connect);
    }

    public static void Connect(PktBinaryRequestInfo packet)
    {
        var sessionID = packet.SessionId;
        DIContainer.Get<UserManager>().AddUser(packet.SessionId);
        DIContainer.Get<RoomManager>().AddUser(packet.SessionId);
    }
}