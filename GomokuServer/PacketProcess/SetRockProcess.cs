using GomokuPacket;

public partial class PacketProcessor
{
    public void SetRockProcess(string SenderSessionId, Packet _packet)
    {
        var packet = (SetRockPacket)_packet;

        var (roomGetResut, room) = RoomManager.GetRoom(SenderSessionId);
        if (roomGetResut != ERROR_CODE.NONE || room is null)
        {
            SendPacket(SenderSessionId, new ErrorPacket() { ErrorCode = roomGetResut });
            return;
        }

        var setRockResult = room.SetRock(SenderSessionId, packet.X, packet.Y);
        if (setRockResult != ERROR_CODE.NONE)
        {
            SendPacket(SenderSessionId, new ErrorPacket() { ErrorCode = setRockResult });
            return;
        }

        // 상대 유저에게 수를 놓았음을 알립니다.
        var otherUser = room.GetOtherUser(SenderSessionId);
        SendPacket(otherUser.SessionId, _packet);

        // 게임이 종료되었으면 모든 유저에게 게임 종료 패킷을 보냅니다.
        // 마지막으로 sender가 수를 놓았는데 게임이 종료되었으므로, sender가 이겼습니다.
        if (room.IsGameEnd())
        {
            SendPacket(SenderSessionId, new GameEndPacket() { AmIWin = true });
            SendPacket(otherUser.SessionId, new GameEndPacket() { AmIWin = false });
        }
    }
}