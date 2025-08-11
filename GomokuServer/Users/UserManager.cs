using GomokuPacket;
using GomokuServer.Config;

namespace GomokuServer.Users;

public static class UserManager
{
    private static Dictionary<string, User> _connectedUsers = new Dictionary<string, User>();

    public static (ErrorCode ErrorCode, User? User) AddUser(string sessionId)
    {
        if (IsFull())
        {
            return (ErrorCode.USER_COUNT_FULL, null);
        }

        if (_connectedUsers.ContainsKey(sessionId))
        {
            return (ErrorCode.USER_ALREADY_EXIST, null);
        }

        var user = new User(sessionId);
        _connectedUsers[sessionId] = user;
        return (ErrorCode.NONE, user);
    }

    public static (ErrorCode ErrorCode, User? User) GetUser(string sessionId)
    {
        if (!_connectedUsers.TryGetValue(sessionId, out var user))
        {
            return (ErrorCode.UNKNOWN_USER, null);
        }

        return (ErrorCode.NONE, user);
    }

    private static bool IsFull()
    {
        var maxUserCount = GameConfig.MaxUserCountPerServer;
        return _connectedUsers.Count >= maxUserCount;
    }
}