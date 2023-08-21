namespace Savor22b.Action.Exceptions;

[Serializable]
public class NotCookingException : ActionException
{
    public NotCookingException(string message, int? errorCode = null)
        : base(message, "NotCooking", errorCode) { }
}
