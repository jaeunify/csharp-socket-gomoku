public class GameOption : Instance
{
    public int MaxUserCountPerServer => MaxRoomCountPerServer * MaxUserCountPerRoom; // 서버당 최대 유저 수
    public int MaxRoomCountPerServer { get; private set; } = 10; // 서버당 최대 방 개수 // TODO 적용
    public int MaxUserCountPerRoom { get; private set; } = 2; // 방당 최대 유저 수 // TODO 적용
    // TODO public static int RoomStartNumber { get; set; } = 0; // 방 시작 번호
    public int BoardSize { get; private set; } = 15; // 바둑판 크기
}