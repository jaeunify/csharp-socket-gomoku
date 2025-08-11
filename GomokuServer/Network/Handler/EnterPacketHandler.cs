using GomokuServer.Manager;
using GomokuPacket;

namespace GomokuServer.Network.Handler;

public class EnterPacketHandler : PacketHandler<EnterPacket>
{
    public EnterPacketHandler(Action<string, Packet> sendPacket) : base(sendPacket) { }
    public override void Handle(string SenderSessionId, EnterPacket packet)
    {
        var (errorCode, user) = UserManager.AddUser(SenderSessionId);
        if (errorCode != ERROR_CODE.NONE || user is null)
        {
            SendPacket(SenderSessionId, new ErrorPacket() { ErrorCode = errorCode });
            return;
        }

        (errorCode, var room) = RoomManager.Enter(user);
        if (errorCode != ERROR_CODE.NONE || room is null)
        {
            SendPacket(SenderSessionId, new ErrorPacket() { ErrorCode = errorCode });
            return;
        }

        // 방이 다 찼으면, 게임을 시작합니다.
        if (room.IsReadyToStart())
        {
            room.Start();

            // 게임 시작 패킷을 두 유저에게 전송합니다.
            var IsMyTurn = room.IsMyTurn(user);
            var otherUser = room.GetOtherUser(user);
            SendPacket(user.SessionId, new GameStartPacket() { AmIFirst = IsMyTurn });
            SendPacket(otherUser.SessionId, new GameStartPacket() { AmIFirst = !IsMyTurn });
        }
    }
}