public class RoomManager
{
    private int RoomIdx = 0;
    private Dictionary<int, Room> Rooms = new Dictionary<int, Room>(); // TODO pooling
    private Dictionary<string, int> SessionIdRoomId = new Dictionary<string, int>(); // session id 로 Room 을 빠르게 찾기 위해 저장
    public void AddUser(User user)
    {
        if (!SessionIdRoomId.ContainsKey(user.SessionId))
        {
            var room = CreateRoom();

            SessionIdRoomId[user.SessionId] = room.RoomId;
            room.Enter(user);

            // todo 고민 해보기
        }
    }

    private Room CreateRoom()
    {
        var room = new Room(++RoomIdx);
        Rooms[room.RoomId] = room;
        return room;
    }
}

public class Room
{
    public int RoomId { get; private set; }
    public bool IsActive { get; set; } = true;
    private Dictionary<string, User> ConnectedUsers = new Dictionary<string, User>();

    public Room(int roomId)
    {
        RoomId = roomId;
    }

    public void Init()
    {
        IsActive = true;
    }

    public void Enter(User user)
    {
        if (!IsActive)
            Init();
            
        ConnectedUsers.Add(user.SessionId, user);
    }

    public void LeaveAll() // todo use
    {
        IsActive = false;
        ConnectedUsers.Clear();
    }
}