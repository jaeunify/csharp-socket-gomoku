using SuperSocketLite.SocketBase;
using SuperSocketLite.SocketBase.Logging;

namespace Gomoku.Network;

// MainServer의 단일 책임 원칙 확립을 위해, 세션 이벤트 핸들링을 별도의 클래스로 분리했습니다.
// 이로 인해 MainServer는 PacketRouter 와의 의존성 없앨 수 있습니다.
public class SessionEventHandler
{
    private readonly ILog Logger;
    private readonly PacketRouter PacketRouter;

    public SessionEventHandler(ILog logger, PacketRouter packetRouter)
    {
        Logger = logger;
        PacketRouter = packetRouter;
    }

    public void OnConnected(NetworkSession session)
    {
        Logger.Debug($"[{DateTime.Now}] 세션 번호 {session.SessionID} 접속 start, ThreadId: {Thread.CurrentThread.ManagedThreadId}");
    }

    public void OnClosed(NetworkSession session, CloseReason reason)
    {
        Logger.Info($"[{DateTime.Now}] 세션 번호 {session.SessionID},  접속해제: {reason.ToString()}");
        // TODO PacketRouter.InsertPacket(퇴장패킷);
    }

    public void RequestReceived(NetworkSession session, PktBinaryRequestInfo reqInfo)
    {
        Logger.Debug($"[{DateTime.Now}] 세션 번호 {session.SessionID},  받은 데이터 크기: {reqInfo.Body.Length}, ThreadId: {Thread.CurrentThread.ManagedThreadId}");
        reqInfo.SessionId = session.SessionID;
        PacketRouter.InsertPacket(reqInfo);
    }
}