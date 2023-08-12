namespace Savor22b.Action.Exceptions;


[Serializable]
public class InvalidFieldIndexException : ActionException
{
    public InvalidFieldIndexException(string message, int? errorCode = null)
        : base(message, "InvalidFieldIndex", errorCode)
    {
    }
}
