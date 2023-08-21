namespace Savor22b.Action.Exceptions;

[Serializable]
public class NotHaveRequiredException : ActionException
{
    public NotHaveRequiredException(string message, int? errorCode = null)
        : base(message, "NotHaveRequired", errorCode)
    {
    }
}
