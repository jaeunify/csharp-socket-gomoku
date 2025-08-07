using GomokuPacket;

public partial class PacketProcessor
{
    public void EnterProcess(string SenderSessionId, Packet _packet)
    {
        var packet = (EnterPacket)_packet;

        var sessionID = SenderSessionId;
        var user = DIContainer.Get<UserManager>().AddUser(sessionID);
        var room = DIContainer.Get<RoomManager>().Enter(user);

        // 방이 다 찼으면, 게임을 시작합니다.
        if (room.IsReadyToStart())
        {
            room.Start();

            var users = room.GetUsers();
            var firstUser = users[0];
            var otherUser = users[1];

            // 선 입장자가 흑돌입니다.
            SendPacket(firstUser.SessionId, new GameStartPacket() { AmIFirst = true });
            SendPacket(otherUser.SessionId, new GameStartPacket() { AmIFirst = false });
        }

    }
}