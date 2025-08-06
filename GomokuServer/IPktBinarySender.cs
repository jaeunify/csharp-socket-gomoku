public interface IPktBinarySender
{
    void Send(string sessionId, byte[] data);
}