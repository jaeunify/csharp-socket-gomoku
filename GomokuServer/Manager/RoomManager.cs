public class RoomManager
{
    private int RoomIdx = 0;
    private Dictionary<int, Room> Rooms = new Dictionary<int, Room>(); // TODO pooling
    private Dictionary<string, int> SessionIdRoomId = new Dictionary<string, int>();
    public void AddUser(string sessionId)
    {
        if (!SessionIdRoomId.ContainsKey(sessionId))
        {
            var room = CreateRoom(sessionId);
            SessionIdRoomId[sessionId] = room.RoomId;
        }
    }

    private Room CreateRoom(string sessionId)
    {
        var room = new Room(++RoomIdx);
        Rooms[room.RoomId] = room;
        SessionIdRoomId[sessionId] = room.RoomId;
        return room;
    }
}

public class Room
{
    public int RoomId { get; private set; }
    public bool IsActive { get; set; } = true;

    public Room(int roomId)
    {
        RoomId = roomId;
    }
}