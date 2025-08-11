namespace GomokuServer.Config;

public static class GameConfig
{
    public static int MaxUserCountPerServer => MaxRoomCountPerServer * 2; // 서버당 최대 유저 수 (오목 서버이니 방당 2명)
    public static int MaxRoomCountPerServer = 10; // 서버당 최대 방 개수 // TODO 적용
    public static int BoardSize = 15; // 바둑판 크기
}