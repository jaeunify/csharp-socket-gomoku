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
        {
            throw new ServerException(ERROR_CODE.UNENTERED_USER);
        }

        if (!Rooms.TryGetValue(roomId, out var room))
        {
            throw new Exception("impossible fatal error: room not found");
        }

        SessionIdRoomId.Remove(user.SessionId); // dic 부터 저장해야, 멀티스레드 환경에서 안전하게 동작함

        room.Leave(user);
        if (room.GetUserCount() <= 0)
        {
            Rooms.Remove(roomId);
        }
    }

    public Room GetRoom(string sesssionId)
    {
        if (!SessionIdRoomId.TryGetValue(sesssionId, out int roomId))
        {
            throw new ServerException(ERROR_CODE.UNENTERED_USER);
        }

        if (!Rooms.TryGetValue(roomId, out var room))
        {
            throw new Exception("impossible fatal error: room not found");
        }

        return room;
    }
}