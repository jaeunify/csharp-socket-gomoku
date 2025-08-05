namespace GomokuPacket;

public enum ERROR_CODE : ushort
{
    NONE = 0, // 에러가 아니다

    USER_COUNT_FULL = 100, // 접속 가능한 유저 수가 가득 찼다
    USER_ALREADY_EXIST = 101, // 이미 동일한 세션 아이디로 접속한 유저가 있다
}