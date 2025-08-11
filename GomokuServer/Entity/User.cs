namespace GomokuServer.Entity;

public class User
{
    private static int _userIdCounter = 0;
    public int UserId { get; private set; }
    public string SessionId { get; private set; }

    public User(string sessionId)
    {
        // 생성된 순서대로 UserId를 부여합니다.
        UserId = _userIdCounter++;
        SessionId = sessionId;
    }
}