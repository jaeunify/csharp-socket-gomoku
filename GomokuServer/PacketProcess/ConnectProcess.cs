using GomokuPacket;

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

            var users = room.GetUsers();
            var firstUser = users[0];
            var otherUsers = users.Skip(1).ToList();

            SendPacket(firstUser.SessionId, new GameStartPacket()
            {
                AmIFirst = true
            });

            foreach (var otherUser in otherUsers)
            {
                SendPacket(otherUser.SessionId, new GameStartPacket()
                {
                    AmIFirst = false
                });
            }
        }
    }
}