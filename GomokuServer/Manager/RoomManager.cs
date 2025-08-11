using GomokuServer.Entity;
using GomokuPacket;

namespace GomokuServer.Manager;

public static class RoomManager
{
    private static Dictionary<int, Room> Rooms = new Dictionary<int, Room>(); // roomId-Room 매핑
    private static Dictionary<int, int> UserIdRoomId = new Dictionary<int, int>(); // userId-RoomId 매핑
    private static Room? PendingRoom = null;

    public static (ERROR_CODE errorCode, Room? room) Enter(User user)
    {
        if (UserIdRoomId.TryGetValue(user.UserId, out var roomId))
        {
            // TODO 재접속
            return (ERROR_CODE.NONE, Rooms[roomId]);
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
            UserIdRoomId[user.UserId] = room.RoomId;

            var enterResult = room.Enter(user);
            if (enterResult != ERROR_CODE.NONE)
            {
                return (enterResult, null);
            }

            return (ERROR_CODE.NONE, room);
        }
    }

    public static ERROR_CODE Leave(User user) // todo use
    {
        if (!UserIdRoomId.TryGetValue(user.UserId, out int roomId))
        {
            return ERROR_CODE.UNENTERED_USER;
        }

        if (!Rooms.TryGetValue(roomId, out var room))
        {
            throw new Exception("impossible fatal error: room not found");
        }

        UserIdRoomId.Remove(user.UserId); // dic 부터 저장해야, 멀티스레드 환경에서 안전하게 동작함

        room.Leave(user);
        if (room.GetUserCount() <= 0)
        {
            Rooms.Remove(roomId);
        }

        return ERROR_CODE.NONE;
    }

    public static (ERROR_CODE errorCode, Room? room) GetRoom(User user)
    {
        if (!UserIdRoomId.TryGetValue(user.UserId, out int roomId))
        {
            return (ERROR_CODE.UNENTERED_USER, null);
        }

        if (!Rooms.TryGetValue(roomId, out var room))
        {
            throw new Exception("impossible fatal error: room not found");
        }

        return (ERROR_CODE.NONE, room);
    }
}