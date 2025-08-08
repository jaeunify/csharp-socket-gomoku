using GomokuPacket;

public class Room
{
    // Room
    private static int RoomIdCounter = 0;
    public int RoomId { get; private set; }
    public bool IsPlaying { get; private set; } = false;
    private Dictionary<string, User> ConnectedUsers = new Dictionary<string, User>(); // sessionId-User 매핑
    private string turnSessionId;

    // Game
    private List<List<int>>? Board;

    public Room()
    {
        RoomId = RoomIdCounter++;
    }

    public void Enter(User user)
    {
        ConnectedUsers[user.SessionId] = user;
    }

    public void Leave(User user)
    {
        ConnectedUsers.Remove(user.SessionId);
    }

    public void Start()
    {
        IsPlaying = true;

        // 보드를 초기화합니다.
        Board = new List<List<int>>();
        var boardSize = DIContainer.Get<GameOption>().BoardSize;
        for (int i = 0; i < boardSize; i++)
        {
            var row = new List<int>();
            for (int j = 0; j < boardSize; j++)
            {
                row.Add(-1);
            }
            Board.Add(row);
        }

        // 선 플레이어를 정합니다.
        var users = GetUsers();
        var random = new Random().Next(0, 2); // 0 또는 1을 랜덤으로 선택
        turnSessionId = users[random].SessionId;
    }

    public bool IsFull()
    {
        return ConnectedUsers.Count >= 2;
    }

    public bool IsReadyToStart()
    {
        return !IsPlaying && IsFull();
    }

    public bool IsMyTurn(string sessionId)
    {
        return turnSessionId == sessionId;
    }

    public List<User> GetUsers()
    {
        return ConnectedUsers.Values.ToList();
    }

    public User GetOtherUser(string sessionId)
    {
        return ConnectedUsers.Values.FirstOrDefault(user => user.SessionId != sessionId)
        ?? throw new Exception("impossible fatal error: other user not found");
    }

    public int GetUserCount()
    {
        return ConnectedUsers.Count;
    }

    /// <summary>
    /// 수를 놓습니다.
    /// </summary>
    /// <returns>게임이 종료되었는지 리턴합니다.</returns>
    public ERROR_CODE SetRock(string sessionId, int x, int y)
    {
        if (IsPlaying == false || Board == null)
        {
            return ERROR_CODE.GAME_UNSTARTED;
        }

        if (!IsMyTurn(sessionId))
        {
            return ERROR_CODE.NOT_MY_TURN;
        }

        if (x < 0 || y < 0 || x >= Board.Count || y >= Board[x].Count)
        {
            return ERROR_CODE.INVALID_ROCK_POSITION;
        }
        var board = Board;

        if (Board[y][x] != -1)
        {
            return ERROR_CODE.ALREADY_SET_ROCK_POSITION;
        }

        Board[y][x] = ConnectedUsers[sessionId].UserId;

        // 다음 턴을 상대 유저로 변경합니다.
        turnSessionId = GetOtherUser(sessionId).SessionId;

        return ERROR_CODE.NONE;
    }

    public bool IsGameEnd()
    {
        if (Board == null)
        {
            return false;
        }
        int size = Board.Count;

        // 4가지 방향 (오른쪽, 아래, 오른쪽아래, 왼쪽아래)
        int[][] directions = new int[][]
        {
        new int[] { 1, 0 },   // →
        new int[] { 0, 1 },   // ↓
        new int[] { 1, 1 },   // ↘
        new int[] { -1, 1 }   // ↙
        };

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int stone = Board[y][x];
                if (stone == -1)
                {
                    continue; // 빈 칸이면 무시
                }

                foreach (var dir in directions)
                {
                    int dx = dir[0];
                    int dy = dir[1];

                    int count = 1;

                    int nx = x + dx;
                    int ny = y + dy;

                    while (nx >= 0 && ny >= 0 && nx < size && ny < size && Board[ny][nx] == stone)
                    {
                        count++;
                        if (count == 5)
                        {
                            return true;
                        }

                        nx += dx;
                        ny += dy;
                    }
                }
            }
        }

        return false;
    }
}