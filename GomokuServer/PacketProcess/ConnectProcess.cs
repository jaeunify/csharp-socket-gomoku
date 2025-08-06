public partial class PacketProcessor
{
    public void ConnectProcess(PktBinaryRequestInfo packet)
    {
        var sessionID = packet.SessionId;
        var user = DIContainer.Get<UserManager>().AddUser(sessionID);
        var room = DIContainer.Get<RoomManager>().Enter(user);
        if (room.IsReadyToStart())
        {
            room.Start();
            // todo send start pacekt to all users in the room
        }
    }
}