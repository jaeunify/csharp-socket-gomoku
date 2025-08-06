using GomokuPacket;

public class ServerException : Exception
{
    public ERROR_CODE ErrorCode { get; } = ERROR_CODE.NONE;

    public ServerException(ERROR_CODE errorCode) : this(errorCode, null) { }

    public ServerException(ERROR_CODE errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
 }