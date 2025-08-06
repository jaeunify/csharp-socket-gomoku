namespace GomokuPacket;

public enum PacketId : Int16
{
    Error = -1,
    None = 0,
    Connect = 1,
    GameStart = 2,
}
