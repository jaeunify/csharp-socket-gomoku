using GomokuPacket;

public class UserManager
{
    private Dictionary<string, User> ConnectedUsers = new Dictionary<string, User>();

    public User AddUser(string sessionId)
    {
        if (IsUserCountFull())
            throw new ServerException(ERROR_CODE.USER_COUNT_FULL);

        if (ConnectedUsers.ContainsKey(sessionId))
            throw new ServerException(ERROR_CODE.USER_ALREADY_EXIST);

        var user = new User(sessionId);
        ConnectedUsers[sessionId] = user;
        return user;
    }

    private bool IsUserCountFull()
    {
        var maxUserCount = DIContainer.Get<GameOption>().MaxUserCountPerServer;
        return ConnectedUsers.Count >= maxUserCount;
    }
}

public class User
{
    public string SessionId { get; private set; }

    public User(string sessionId)
    {
        SessionId = sessionId;
    }
}