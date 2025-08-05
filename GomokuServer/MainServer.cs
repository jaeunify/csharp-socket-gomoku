using SuperSocketLite.SocketBase;
using SuperSocketLite.SocketBase.Protocol;
using SuperSocketLite.SocketBase.Config;



class MainServer : AppServer<NetworkSession, PktBinaryRequestInfo>
{

    public MainServer()
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, PktBinaryRequestInfo>())
    {
        NewSessionConnected += new SessionHandler<NetworkSession>(OnConnected);
        SessionClosed += new SessionHandler<NetworkSession, CloseReason>(OnClosed);
        NewRequestReceived += new RequestHandler<NetworkSession, PktBinaryRequestInfo>(RequestReceived);
    }

    public void CreateServer()
    {
        try
        {
            var config = new ServerConfig
            {
                Port = 32452,
                Ip = "Any",
                MaxConnectionNumber = 100,
                Mode = SocketMode.Tcp,
                Name = "Echo Server"
            };

            bool bResult = Setup(new RootConfig(), config, logFactory: null);

            if (bResult == false)
            {
                Console.WriteLine("[ERROR] 서버 네트워크 설정 실패 ㅠㅠ");
                return;
            }

            Logger.Info($"[{DateTime.Now}] 서버 생성 성공");
        }
        catch (Exception ex)
        {
            Logger.Error($"서버 생성 실패: {ex.ToString()}");
        }
    }


    void OnConnected(NetworkSession session)
    {
        Logger.Debug($"[{DateTime.Now}] 세션 번호 {session.SessionID} 접속 start, ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
    }

    void OnClosed(NetworkSession session, CloseReason reason)
    {
        Logger.Info($"[{DateTime.Now}] 세션 번호 {session.SessionID},  접속해제: {reason.ToString()}");
    }

    void RequestReceived(NetworkSession session, PktBinaryRequestInfo reqInfo)
    {
        Logger.Debug($"[{DateTime.Now}] 세션 번호 {session.SessionID},  받은 데이터 크기: {reqInfo.Body.Length}, ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");


        var totalSize = (Int16)(reqInfo.Body.Length + PktBinaryRequestInfo.HEADERE_SIZE);

        List<byte> dataSource = new List<byte>();
        dataSource.AddRange(BitConverter.GetBytes(totalSize));
        dataSource.AddRange(BitConverter.GetBytes((Int16)reqInfo.PacketID));
        dataSource.AddRange(reqInfo.Body);

        session.Send(dataSource.ToArray(), 0, dataSource.Count);
    }
}


public class NetworkSession : AppSession<NetworkSession, PktBinaryRequestInfo>
{
}

