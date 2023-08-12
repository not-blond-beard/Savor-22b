namespace Savor22b.Action.Exceptions;


[Serializable]
public class NotFoundDataException : ActionException
{
    public NotFoundDataException(string message, int? errorCode = null)
        : base(message, "NotFoundData", errorCode)
    {
    }
}
