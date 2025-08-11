using GomokuPacket;

public class EnterPacketHandler : PacketHandler<EnterPacket>
{
    public EnterPacketHandler(Action<string, Packet> sendPacket) : base(sendPacket) { }
    public override void Handle(string SenderSessionId, EnterPacket packet)
    {
        var sessionID = SenderSessionId;

        var (errorCode, user) = UserManager.AddUser(sessionID);
        if (errorCode != ERROR_CODE.NONE || user is null)
        {
            SendPacket(sessionID, new ErrorPacket() { ErrorCode = errorCode });
            return;
        }

        var room = RoomManager.Enter(user);

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