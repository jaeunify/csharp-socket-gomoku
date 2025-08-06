public interface IPacketSender
{
    void Send(string sessionId, byte[] data);
}