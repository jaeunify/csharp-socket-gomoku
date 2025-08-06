using GomokuPacket;

public class UserManager
{
    private HashSet<string> ConnectedSessionIds = new HashSet<string>();

    public ERROR_CODE AddUser(string sessionId)
    {
        if (IsUserCountFull())
        {
            return ERROR_CODE.USER_COUNT_FULL;
        }

        if (ConnectedSessionIds.Contains(sessionId))
        {
            return ERROR_CODE.USER_ALREADY_EXIST;
        }

        ConnectedSessionIds.Add(sessionId);

        return ERROR_CODE.NONE;
    }

    private bool IsUserCountFull()
    {
        var maxUserCount = DIContainer.Get<GameOption>().MaxUserCountPerServer;
        return ConnectedSessionIds.Count >= maxUserCount;
    }
}