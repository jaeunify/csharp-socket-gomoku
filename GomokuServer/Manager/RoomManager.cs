using GomokuPacket;

public class RoomManager
{
    private Dictionary<int, Room> Rooms = new Dictionary<int, Room>();
    private Dictionary<string, int> SessionIdRoomId = new Dictionary<string, int>(); // session id 로 Room 을 빠르게 찾기 위해 저장
    private Pool<Room> RoomPool = new Pool<Room>();
    public void Enter(User user)
    {
        if (!SessionIdRoomId.ContainsKey(user.SessionId))
        {
            var room = RoomPool.RentFromPool();
            Rooms[room.RoomId] = room; // dic 부터 저장해야, 멀티스레드 환경에서 안전하게 동작함
            SessionIdRoomId[user.SessionId] = room.RoomId;
            room.Enter(user);
        }
    }

    public void Leave(User user) // todo use
    {
        if (!SessionIdRoomId.TryGetValue(user.SessionId, out int roomId))
            throw new ServerException(ERROR_CODE.UNKNOWN_USER);

        if (!Rooms.TryGetValue(roomId, out Room room))
            throw new Exception("impossible fatal error: room not found");

        SessionIdRoomId.Remove(user.SessionId); // dic 부터 저장해야, 멀티스레드 환경에서 안전하게 동작함

        var isRoomDeactivated = room.Leave(user);
        if (isRoomDeactivated)
        {
            Rooms.Remove(roomId);
            RoomPool.ReturnToPool(room);
        }
    }
}

public class Room
{
    private static int RoomIdx = 0;
    public int RoomId { get; private set; }
    public bool IsActive { get; set; } = true; // todo 삭제 검토
    private Dictionary<string, User> ConnectedUsers = new Dictionary<string, User>();

    public Room()
    {
        RoomId = ++RoomIdx;
    }

    public void Init()
    {
        IsActive = true;
    }

    public void Enter(User user)
    {
        // 활성화
        if (!IsActive)
            Init();

        ConnectedUsers.Add(user.SessionId, user);
    }

    /// <returns>bool isRoomDeactivated: 해당 방이 비활성화되면 true를 리턴합니다.</returns>
    public bool Leave(User user)
    {
        ConnectedUsers.Remove(user.SessionId);

        // 비활성화
        if (ConnectedUsers.Count == 0)
        {
            IsActive = false;
            return true;
        }

        return false;
    }
}