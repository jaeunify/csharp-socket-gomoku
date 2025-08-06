public partial class PacketProcessor
{
    public void ConnectProcess(PktBinaryRequestInfo packet)
    {
        var sessionID = packet.SessionId;
        var user = DIContainer.Get<UserManager>().AddUser(sessionID);
        DIContainer.Get<RoomManager>().Enter(user);
    }
}