using GomokuPacket;

public partial class PacketProcessor
{
    public void EnterProcess(Packet _packet)
    {
        var packet = (EnterPacket)_packet;

        var sessionID = packet.SenderSessionId;
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