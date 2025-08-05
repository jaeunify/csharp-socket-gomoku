public static class GameOption
{
    public static int MaxUserCountPerServer => MaxRoomCountPerServer * MaxUserCountPerRoom; // 서버당 최대 유저 수
    public static int MaxRoomCountPerServer { get; set; } = 10; // 서버당 최대 방 개수 // TODO 적용
    public static int MaxUserCountPerRoom { get; set; } = 2; // 방당 최대 유저 수 // TODO 적용
    // TODO public static int RoomStartNumber { get; set; } = 0; // 방 시작 번호
}