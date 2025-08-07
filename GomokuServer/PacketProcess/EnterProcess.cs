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

            var otherUser = room.GetOtherUser(sessionID);
            var IsMyTurn = room.IsMyTurn(sessionID);
            SendPacket(user.SessionId, new GameStartPacket() { AmIFirst = IsMyTurn });
            SendPacket(otherUser.SessionId, new GameStartPacket() { AmIFirst = !IsMyTurn });
        }

    }
}