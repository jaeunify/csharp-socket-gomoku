using GomokuServer.Entity;
using GomokuPacket;

namespace GomokuServer.Manager;

public static class RoomManager
{
    private static Dictionary<int, Room> _Rooms = new Dictionary<int, Room>(); // roomId-Room 매핑
    private static Dictionary<int, int> _UserIdRoomId = new Dictionary<int, int>(); // userId-RoomId 매핑
    private static Room? _PendingRoom = null;

    public static (ERROR_CODE errorCode, Room room) Enter(User user)
    {
        if (_UserIdRoomId.TryGetValue(user.UserId, out var roomId))
        {
            // TODO 재접속
            return (ERROR_CODE.NONE, _Rooms[roomId]);
        }
        else
        {
            Room room;
            if (_PendingRoom is null)
            {
                // 내가 첫 입장인 경우, 방을 생성하고 PendingRoom에 등록
                room = new Room();
                _PendingRoom = room; 
            }
            else
            {
                // 내가 두 번째 입장인 경우, PendingRoom을 가져와서 사용, 초기화
                room = _PendingRoom;
                _PendingRoom = null;
            }

            // dic 부터 저장해야, 멀티스레드 환경에서 안전하게 동작함
            _Rooms[room.RoomId] = room;
            _UserIdRoomId[user.UserId] = room.RoomId;

            return (room.Enter(user), room);
        }
    }

    public static ERROR_CODE Leave(User user) // todo use
    {
        if (!_UserIdRoomId.TryGetValue(user.UserId, out int roomId))
        {
            return ERROR_CODE.UNENTERED_USER;
        }

        if (!_Rooms.TryGetValue(roomId, out var room))
        {
            throw new Exception("impossible fatal error: room not found");
        }

        _UserIdRoomId.Remove(user.UserId); // dic 부터 저장해야, 멀티스레드 환경에서 안전하게 동작함

        room.Leave(user);
        if (room.GetUserCount() <= 0)
        {
            _Rooms.Remove(roomId);
        }

        return ERROR_CODE.NONE;
    }

    public static (ERROR_CODE errorCode, Room? room) GetRoom(User user)
    {
        if (!_UserIdRoomId.TryGetValue(user.UserId, out int roomId))
        {
            return (ERROR_CODE.UNENTERED_USER, null);
        }

        if (!_Rooms.TryGetValue(roomId, out var room))
        {
            throw new Exception("impossible fatal error: room not found");
        }

        return (ERROR_CODE.NONE, room);
    }
}