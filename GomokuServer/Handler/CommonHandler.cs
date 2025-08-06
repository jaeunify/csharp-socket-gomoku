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
        var user = DIContainer.Get<UserManager>().AddUser(sessionID);
        DIContainer.Get<RoomManager>().AddUser(user);
    }
}