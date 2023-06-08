namespace Savor22b.Action;

using System.Runtime.Serialization;


[Serializable]
public class NotFoundTableDataException : Exception, ISerializable
{
    public NotFoundTableDataException()
    {
    }

    public NotFoundTableDataException(string? message)
        : base(message)
    {
    }

    public NotFoundTableDataException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
