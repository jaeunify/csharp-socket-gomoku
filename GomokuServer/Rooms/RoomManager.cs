using GomokuPacket;
using GomokuServer.Users;

namespace GomokuServer.Rooms;

public class RoomManager : Manager
{
    private Dictionary<int, Room> _rooms = new Dictionary<int, Room>(); // roomId-Room 매핑
    private Dictionary<int, int> _userIdRoomId = new Dictionary<int, int>(); // userId-RoomId 매핑
    private Room? _pendingRoom = null;

    public Room GetRoom(User user)
    {
        if (_userIdRoomId.TryGetValue(user.UserId, out var roomId))
        {
            return _rooms[roomId];
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

            return room;
        }
    }
}