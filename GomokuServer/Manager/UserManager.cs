using GomokuServer.Entity;
using GomokuPacket;

namespace GomokuServer.Manager;

public static class UserManager
{
    private static Dictionary<string, User> _ConnectedUsers = new Dictionary<string, User>();

    public static (ERROR_CODE ErrorCode, User? User) AddUser(string sessionId)
    {
        if (IsFull())
        {
            return (ERROR_CODE.USER_COUNT_FULL, null);
        }

        if (_ConnectedUsers.ContainsKey(sessionId))
        {
            return (ERROR_CODE.USER_ALREADY_EXIST, null);
        }

        var user = new User(sessionId);
        _ConnectedUsers[sessionId] = user;
        return (ERROR_CODE.NONE, user);
    }

    public static (ERROR_CODE ErrorCode, User? User) GetUser(string sessionId)
    {
        if (!_ConnectedUsers.TryGetValue(sessionId, out var user))
        {
            return (ERROR_CODE.UNKNOWN_USER, null);
        }

        return (ERROR_CODE.NONE, user);
    }

    private static bool IsFull()
    {
        var maxUserCount = GameOption.MaxUserCountPerServer;
        return _ConnectedUsers.Count >= maxUserCount;
    }
}