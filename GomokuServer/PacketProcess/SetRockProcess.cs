using GomokuPacket;

public partial class PacketProcessor
{
    public void SetRockProcess(string SenderSessionId, Packet _packet)
    {
        var packet = (SetRockPacket)_packet;

        var room = DIContainer.Get<RoomManager>().GetRoom(SenderSessionId);
        var IsGameEnd = room.SetRock(SenderSessionId, packet.X, packet.Y);

        var otherUser = room.GetOtherUser(SenderSessionId);

        // 상대 유저에게 수를 놓았음을 알립니다.
        SendPacket(otherUser.SessionId, _packet);

        // 게임이 종료되었으면 모든 유저에게 게임 종료 패킷을 보냅니다.
        // 마지막으로 sender가 수를 놓았는데 게임이 종료되었으므로, sender가 이겼습니다.
        if (IsGameEnd)
        {
            SendPacket(SenderSessionId, new GameEndPacket() { AmIWin = true });
            SendPacket(otherUser.SessionId, new GameEndPacket() { AmIWin = false });
        }
    }
}