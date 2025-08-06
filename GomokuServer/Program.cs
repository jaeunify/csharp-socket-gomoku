// EchoServer.csproj 에서 default namespace 를 EchoServer 로 설정했습니다. -> namespace 생략

using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello SuperSocketLite");

        var services = new ServiceCollection();
        services.AddSingleton<GameOption>();
        services.AddSingleton<ServerOption>();
        services.AddSingleton<UserManager>();
        services.AddSingleton<RoomManager>();
        var provider = services.BuildServiceProvider();
        DIContainer.Init(provider);

        var server = new MainServer();
        server.Create();
        var IsResult = server.Start();

        if (IsResult)
        {
            server.Logger.Info("서버 네트워크 시작");
        }
        else
        {
            Console.WriteLine("서버 네트워크 시작 실패");
            return;
        }

        Console.WriteLine("key를 누르면 종료한다....");
        Console.ReadKey();

        server.Stop();
    }
}