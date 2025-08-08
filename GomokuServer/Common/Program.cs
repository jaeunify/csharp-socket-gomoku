// EchoServer.csproj 에서 default namespace 를 EchoServer 로 설정했습니다. -> namespace 생략

using Microsoft.Extensions.DependencyInjection;
using SuperSocketLite.SocketBase;
using SuperSocketLite.SocketBase.Config;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello SuperSocketLite");

        // 서버 생성, 설정
        var server = BootMainServer();

        // 패킷 프로세서 생성 및 대기
        var packetProcessor = new PacketProcessor(server.Send);
        packetProcessor.CreateAndStart();

        var eventHandler = new SessionEventHandler(server.Logger, packetProcessor);

        StartServer(server, eventHandler);

        Console.WriteLine("key를 누르면 종료한다....");
        Console.ReadKey();

        server.Stop();
        packetProcessor.Destroy();
    }

    public static MainServer BootMainServer()
    {
        var server = new MainServer();

        var config = new ServerConfig
        {
            Port = 32452,
            Ip = "Any",
            MaxConnectionNumber = GameOption.MaxUserCountPerServer,
            Mode = SocketMode.Tcp,
            Name = "Gomoku Server"
        };

        bool bResult = server.Setup(new RootConfig(), config, logFactory: null);

        if (!bResult)
        {
            throw new Exception("서버 네트워크 설정 실패");
        }

        server.Logger.Info($"[{DateTime.Now}] 서버 생성 성공");

        return server;
    }

    public static void StartServer(MainServer server, SessionEventHandler eventHandler)
    {
        server.NewSessionConnected += new SessionHandler<NetworkSession>(eventHandler.OnConnected);
        server.SessionClosed += new SessionHandler<NetworkSession, CloseReason>(eventHandler.OnClosed);
        server.NewRequestReceived += new RequestHandler<NetworkSession, PktBinaryRequestInfo>(eventHandler.RequestReceived);

        var IsServerStartSuccess = server.Start();

        if (!IsServerStartSuccess) // server가 이미 Start 된 상태일 경우
        {
            throw new Exception("서버 네트워크 시작 실패");
        }

        server.Logger.Info("서버 네트워크 시작");
    }
}