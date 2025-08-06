using GomokuPacket;

public class RoomManager
{
    private Dictionary<int, Room> Rooms = new Dictionary<int, Room>();
    private Dictionary<string, int> SessionIdRoomId = new Dictionary<string, int>(); // session id 로 Room 을 빠르게 찾기 위해 저장
    private Pool<Room> RoomPool = new Pool<Room>();
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
            var hasPendingRoom = PendingRoom is not null;
            var room = hasPendingRoom ? PendingRoom : RoomPool.RentFromPool();

            if (hasPendingRoom)
            {
                PendingRoom = null; // 2명이 채워졌으니, PendingRoom을 비워줌
            }
            else
            {
                PendingRoom = room; // 내가 첫 입장이니, 다음 입장할 유저를 위해 PendingRoom에 저장
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
            throw new ServerException(ERROR_CODE.UNKNOWN_USER);

        if (!Rooms.TryGetValue(roomId, out Room room))
            throw new Exception("impossible fatal error: room not found");

        SessionIdRoomId.Remove(user.SessionId); // dic 부터 저장해야, 멀티스레드 환경에서 안전하게 동작함

        room.Leave(user);
        if (room.RoomState == RoomState.Deactivated)
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
    public RoomState RoomState { get; private set; }
    private Dictionary<string, User> ConnectedUsers = new Dictionary<string, User>();

    public Room()
    {
        RoomId = ++RoomIdx;
    }

    public void Enter(User user)
    {
        // 활성화
        if (RoomState == RoomState.Deactivated)
            RoomState = RoomState.Waiting;

        ConnectedUsers.Add(user.SessionId, user);
    }

    public void Leave(User user)
    {
        ConnectedUsers.Remove(user.SessionId);

        // 비활성화
        if (ConnectedUsers.Count == 0)
            RoomState = RoomState.Deactivated;
    }

    public void Start()
    { 
        RoomState = RoomState.Playing;
    }

    public bool IsFull()
    {
        return ConnectedUsers.Count >= DIContainer.Get<GameOption>().MaxUserCountPerRoom;
    }

    public bool IsReadyToStart()
    {
        return RoomState <= RoomState.Waiting && IsFull();
    }
}

public enum RoomState
{
    Deactivated,
    Waiting,
    Playing
}