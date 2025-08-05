using GomokuProtocol;

public class UserManager // TODO singleton 으로 개선
{
    UInt64 UserSeq = 0;
    private Dictionary<string, User> UserMap = new Dictionary<string, User>();

    GameOption GameOption { get; set; }

    public UserManager(GameOption gameOption)
    {
        GameOption = gameOption;
    }

    public ERROR_CODE AddUser(string userId, string sessionId) // TODO 어디서 호출하지?
    {
        if (IsUserCountFull())
        {
            return ERROR_CODE.USER_COUNT_FULL;
        }

        if (UserMap.ContainsKey(sessionId))
        {
            return ERROR_CODE.USER_ALREADY_EXIST;
        }

        UserSeq++; // TODO 어디다쓰지 ?

        var user = new User { UserId = userId, SessionId = sessionId };
        UserMap.Add(sessionId, user);

        return ERROR_CODE.NONE;
    }

    private bool IsUserCountFull()
    {
        return UserMap.Count >= GameOption.MaxUserCountPerServer;
    }
}
public class User
{
    public string UserId { get; set; }
    public string SessionId { get; set; }

}