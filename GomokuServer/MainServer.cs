using SuperSocketLite.SocketBase;
using SuperSocketLite.SocketBase.Protocol;
using SuperSocketLite.SocketBase.Config;
using GomokuPacket;

class MainServer : AppServer<NetworkSession, PktBinaryRequestInfo>, IPktBinarySender
{
    private PacketProcessor PacketProcessor { get; set; }

    public MainServer()
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, PktBinaryRequestInfo>())
    {
        NewSessionConnected += new SessionHandler<NetworkSession>(OnConnected);
        SessionClosed += new SessionHandler<NetworkSession, CloseReason>(OnClosed);
        NewRequestReceived += new RequestHandler<NetworkSession, PktBinaryRequestInfo>(RequestReceived);
    }

    public void Create()
    {
        try
        {
            var config = new ServerConfig
            {
                Port = 32452,
                Ip = "Any",
                MaxConnectionNumber = DIContainer.Get<GameOption>().MaxUserCountPerServer,
                Mode = SocketMode.Tcp,
                Name = "Gomoku Server"
            };

            bool bResult = Setup(new RootConfig(), config, logFactory: null);

            if (bResult == false)
            {
                Console.WriteLine("[ERROR] 서버 네트워크 설정 실패 ㅠㅠ");
                return;
            }

            PacketProcessor = new PacketProcessor(this);
            PacketProcessor.CreateAndStart();

            Logger.Info($"[{DateTime.Now}] 서버 생성 성공");
        }
        catch (Exception ex)
        {
            Logger.Error($"서버 생성 실패: {ex.ToString()}");
        }
    }

    public override void Stop()
    {
        base.Stop();
        PacketProcessor.Destroy();
    }



    void OnConnected(NetworkSession session)
    {
        Logger.Debug($"[{DateTime.Now}] 세션 번호 {session.SessionID} 접속 start, ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
    }

    void OnClosed(NetworkSession session, CloseReason reason)
    {
        Logger.Info($"[{DateTime.Now}] 세션 번호 {session.SessionID},  접속해제: {reason.ToString()}");

        // TODO PacketProcessor.InsertPacket(퇴장패킷);
    }

    void RequestReceived(NetworkSession session, PktBinaryRequestInfo reqInfo)
    {
        Logger.Debug($"[{DateTime.Now}] 세션 번호 {session.SessionID},  받은 데이터 크기: {reqInfo.Body.Length}, ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

        reqInfo.SessionId = session.SessionID;

        PacketProcessor.InsertPacket(reqInfo);
    }

    public void Send(string sessionId, byte[] data)
    {
        var session = GetSessionByID(sessionId);

        try
        {
            if (session == null)
            {
                return;
            }

            session.Send(data, 0, data.Length);
        }
        catch (Exception)
        {
            session.SendEndWhenSendingTimeOut();
            session.Close();
        }
    }
}


public class NetworkSession : AppSession<NetworkSession, PktBinaryRequestInfo>
{
}

