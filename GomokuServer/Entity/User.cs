namespace GomokuServer.Entity;

public class User
{
    private static int UserIdCounter = 0;
    public int UserId { get; private set; }
    public string SessionId { get; private set; }

    public User(string sessionId)
    {
        UserId = UserIdCounter++;
        SessionId = sessionId;
    }
}