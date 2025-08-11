using GomokuServer.Entity;
using GomokuPacket;

namespace GomokuServer.Manager;

public static class RoomManager
{
    private static Dictionary<int, Room> _rooms = new Dictionary<int, Room>(); // roomId-Room 매핑
    private static Dictionary<int, int> _userIdRoomId = new Dictionary<int, int>(); // userId-RoomId 매핑
    private static Room? _pendingRoom = null;

    public static (ERROR_CODE errorCode, Room room) Enter(User user)
    {
        if (_userIdRoomId.TryGetValue(user.UserId, out var roomId))
        {
            // TODO 재접속
            return (ERROR_CODE.NONE, _rooms[roomId]);
        }
        else
        {
            Room room;
            if (_pendingRoom is null)
            {
                // 내가 첫 입장인 경우, 방을 생성하고 PendingRoom에 등록
                room = new Room();
                _pendingRoom = room; 
            }
            else
            {
                // 내가 두 번째 입장인 경우, PendingRoom을 가져와서 사용, 초기화
                room = _pendingRoom;
                _pendingRoom = null;
            }

            // dic 부터 저장해야, 멀티스레드 환경에서 안전하게 동작함
            _rooms[room.RoomId] = room;
            _userIdRoomId[user.UserId] = room.RoomId;

            return (room.Enter(user), room);
        }
    }

    public static ERROR_CODE Leave(User user) // todo use
    {
        if (!_userIdRoomId.TryGetValue(user.UserId, out int roomId))
        {
            return ERROR_CODE.UNENTERED_USER;
        }

        if (!_rooms.TryGetValue(roomId, out var room))
        {
            throw new Exception("impossible fatal error: room not found");
        }

        _userIdRoomId.Remove(user.UserId); // dic 부터 저장해야, 멀티스레드 환경에서 안전하게 동작함

        room.Leave(user);
        if (room.GetUserCount() <= 0)
        {
            _rooms.Remove(roomId);
        }

        return ERROR_CODE.NONE;
    }

    public static (ERROR_CODE errorCode, Room? room) GetRoom(User user)
    {
        if (!_userIdRoomId.TryGetValue(user.UserId, out int roomId))
        {
            return (ERROR_CODE.UNENTERED_USER, null);
        }

        if (!_rooms.TryGetValue(roomId, out var room))
        {
            throw new Exception("impossible fatal error: room not found");
        }

        return (ERROR_CODE.NONE, room);
    }
}