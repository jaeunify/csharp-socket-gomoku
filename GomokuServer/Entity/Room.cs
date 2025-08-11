using GomokuPacket;

namespace GomokuServer.Entity;

public class Room
{
    private static int RoomIdCounter = 0;

    public int RoomId { get; private set; }
    public bool IsPlaying { get; private set; } = false;

    private Dictionary<int, User> _ConnectedUsers = new Dictionary<int, User>(); // userId-User 매핑
    private User? _nowTurnUser = null;
    private List<List<int>>? _Board;

    public Room()
    {
        // 생성된 순서대로 RoomId를 부여합니다.
        RoomId = RoomIdCounter++;
    }

    public ERROR_CODE Enter(User user)
    {
        if (IsFull())
        {
            return ERROR_CODE.USER_COUNT_FULL;
        }

        _ConnectedUsers[user.UserId] = user;
        return ERROR_CODE.NONE;
    }

    public void Leave(User user)
    {
        _ConnectedUsers.Remove(user.UserId);
    }

    public void Start()
    {
        IsPlaying = true;

        // 보드를 초기화합니다.
        _Board = new List<List<int>>();
        var boardSize = GameOption.BoardSize;
        for (int i = 0; i < boardSize; i++)
        {
            var row = new List<int>();
            for (int j = 0; j < boardSize; j++)
            {
                row.Add(-1);
            }
            _Board.Add(row);
        }

        // 선 플레이어를 정합니다.
        var users = GetUsers();
        var random = new Random().Next(0, 2); // 0 또는 1을 랜덤으로 선택
        _nowTurnUser = users[random];
    }

    /// <summary>
    /// 수를 놓습니다.
    /// </summary>
    /// <returns>게임이 종료되었는지 리턴합니다.</returns>
    public ERROR_CODE SetRock(User user, int x, int y)
    {
        if (IsPlaying == false || _Board == null)
        {
            return ERROR_CODE.GAME_UNSTARTED;
        }

        if (!IsMyTurn(user))
        {
            return ERROR_CODE.NOT_MY_TURN;
        }

        if (x < 0 || y < 0 || x >= _Board.Count || y >= _Board[x].Count)
        {
            return ERROR_CODE.INVALID_ROCK_POSITION;
        }
        var board = _Board;

        if (_Board[y][x] != -1)
        {
            return ERROR_CODE.ALREADY_SET_ROCK_POSITION;
        }

        _Board[y][x] = user.UserId;

        // 다음 턴을 상대 유저로 변경합니다.
        _nowTurnUser = GetOtherUser(user);

        return ERROR_CODE.NONE;
    }

    public bool IsGameEnd()
    {
        if (_Board == null)
        {
            return false;
        }
        int size = _Board.Count;

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
                int stone = _Board[y][x];
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

                    while (nx >= 0 && ny >= 0 && nx < size && ny < size && _Board[ny][nx] == stone)
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

    public bool IsFull()
    {
        return GetUserCount() >= 2;
    }

    public bool IsReadyToStart()
    {
        return !IsPlaying && IsFull();
    }

    public bool IsMyTurn(User user)
    {
        return _nowTurnUser == user;
    }

    public List<User> GetUsers()
    {
        return _ConnectedUsers.Values.ToList();
    }

    public User GetOtherUser(User user)
    {
        return _ConnectedUsers.Values.FirstOrDefault(u => u != user)
        ?? throw new Exception("impossible fatal error: other user not found");
    }

    public int GetUserCount()
    {
        return _ConnectedUsers.Count;
    }
}