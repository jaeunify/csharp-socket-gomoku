using GomokuPacket;

public class UserManager
{
    private Dictionary<string, User> ConnectedUsers = new Dictionary<string, User>();

    public (ERROR_CODE ErrorCode, User? User) AddUser(string sessionId)
    {
        if (IsUserCountFull())
            return (ERROR_CODE.USER_COUNT_FULL, null);

        if (ConnectedUsers.ContainsKey(sessionId))
            return (ERROR_CODE.USER_ALREADY_EXIST, null);

        var user = new User(sessionId);
        ConnectedUsers[sessionId] = user;
        return (ERROR_CODE.NONE, user);
    }

    public (ERROR_CODE ErrorCode, User? User) GetUser(string sessionId) // todo 삭제 고려
    {
        if (!ConnectedUsers.TryGetValue(sessionId, out var user))
            return (ERROR_CODE.UNKNOWN_USER, null);

        return (ERROR_CODE.NONE, user);
    }

    private bool IsUserCountFull()
    {
        var maxUserCount = DIContainer.Get<GameOption>().MaxUserCountPerServer;
        return ConnectedUsers.Count >= maxUserCount;
    }
}