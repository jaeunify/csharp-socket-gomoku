using GomokuServer.Users;
using GomokuServer.Rooms;
using GomokuPacket;

namespace GomokuServer.Handlers;

public class SetRockHandler : PacketHandler<SetRockPacket>
{
    public SetRockHandler(Action<string, Packet> sendPacket) : base(sendPacket) { }

    public override void Handle(string SenderSessionId, SetRockPacket packet)
    {
        var (errorCode, user) = UserManager.GetUser(SenderSessionId);
        if (errorCode != ErrorCode.NONE || user is null)
        {
            SendPacket(SenderSessionId, new ErrorPacket() { ErrorCode = errorCode });
            return;
        }

        (errorCode, var room) = RoomManager.GetRoom(user);
        if (errorCode != ErrorCode.NONE || room is null)
        {
            SendPacket(SenderSessionId, new ErrorPacket() { ErrorCode = errorCode });
            return;
        }

        errorCode = room.SetRock(user, packet.X, packet.Y);
        if (errorCode != ErrorCode.NONE)
        {
            SendPacket(SenderSessionId, new ErrorPacket() { ErrorCode = errorCode });
            return;
        }

        // 상대 유저에게 수를 놓았음을 알립니다.
        var otherUser = room.GetOtherUser(user);
        SendPacket(otherUser.SessionId, packet);

        // 게임이 종료되었으면 모든 유저에게 게임 종료 패킷을 보냅니다.
        // 마지막으로 sender가 수를 놓았는데 게임이 종료되었으므로, sender가 이겼습니다.
        if (room.IsGameEnd())
        {
            SendPacket(SenderSessionId, new GameEndPacket() { AmIWin = true });
            SendPacket(otherUser.SessionId, new GameEndPacket() { AmIWin = false });
        }
    }
}