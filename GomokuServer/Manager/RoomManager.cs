using GomokuPacket;

public class RoomManager
{
    private Dictionary<int, Room> Rooms = new Dictionary<int, Room>(); // roomId-Room 매핑
    private Dictionary<string, int> SessionIdRoomId = new Dictionary<string, int>(); // sessionId-RoomId 매핑
    private Room? PendingRoom = null;

    public Room Enter(User user)
    {
        if (SessionIdRoomId.TryGetValue(user.SessionId, out var roomId))
        {
            // TODO 재접속
            return Rooms[roomId];
        }
        else
        {
            Room room;
            if (PendingRoom is null)
            {
                room = new Room();
                PendingRoom = room; // 내가 첫 입장이니, 다음 입장할 유저를 위해 PendingRoom에 저장
            }
            else
            {
                room = PendingRoom;
                PendingRoom = null;
            }

            // dic 부터 저장해야, 멀티스레드 환경에서 안전하게 동작함
            Rooms[room.RoomId] = room;
            SessionIdRoomId[user.SessionId] = room.RoomId;

            room.Enter(user);
            return room;
        }
    }

    public void Leave(User user) // todo use
    {
        if (!SessionIdRoomId.TryGetValue(user.SessionId, out int roomId))
            throw new ServerException(ERROR_CODE.UNENTERED_USER);

        if (!Rooms.TryGetValue(roomId, out var room))
            throw new Exception("impossible fatal error: room not found");

        SessionIdRoomId.Remove(user.SessionId); // dic 부터 저장해야, 멀티스레드 환경에서 안전하게 동작함

        room.Leave(user);
        if (room.GetUserCount() <= 0)
            Rooms.Remove(roomId);
    }

    public Room GetRoom(string sesssionId)
    {
        if (!SessionIdRoomId.TryGetValue(sesssionId, out int roomId))
            throw new ServerException(ERROR_CODE.UNENTERED_USER);

        if (!Rooms.TryGetValue(roomId, out var room))
            throw new Exception("impossible fatal error: room not found");

        return room;
    }
}

public class Room
{
    // Room
    private static int RoomIdx = 0;
    public int RoomId { get; private set; }
    public bool IsPlaying { get; private set; } = false;
    private Dictionary<string, User> ConnectedUsers = new Dictionary<string, User>(); // sessionId-User 매핑

    // Game
    private List<List<int>>? Board;

    public Room()
    {
        RoomId = RoomIdx++;
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
    }

    public bool IsFull()
    {
        return ConnectedUsers.Count >= 2;
    }

    public bool IsReadyToStart()
    {
        return !IsPlaying && IsFull();
    }

    public List<User> GetUsers()
    {
        return ConnectedUsers.Values.ToList();
    }

    public User? GetOtherUser(string sessionId)
    {
        return ConnectedUsers.Values.FirstOrDefault(user => user.SessionId != sessionId);
    }

    public int GetUserCount()
    {
        return ConnectedUsers.Count;
    }

    /// <summary>
    /// 수를 놓습니다.
    /// </summary>
    /// <returns>게임이 종료되었는지 리턴합니다.</returns>
    public bool SetRock(string sessionId, int x, int y)
    {
        if (IsPlaying == false)
            throw new ServerException(ERROR_CODE.GAME_UNSTARTED);

        return IsGameEnd();
    }

    public bool IsGameEnd()
    {
        return false;
    }
}