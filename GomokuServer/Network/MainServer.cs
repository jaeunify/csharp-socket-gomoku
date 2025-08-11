using SuperSocketLite.SocketBase;
using SuperSocketLite.SocketBase.Protocol;

namespace GomokuServer.Network;

class MainServer : AppServer<NetworkSession, PktBinaryRequestInfo>
{
    public MainServer() : base(new DefaultReceiveFilterFactory<ReceiveFilter, PktBinaryRequestInfo>()) { }

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


public class NetworkSession : AppSession<NetworkSession, PktBinaryRequestInfo> { }