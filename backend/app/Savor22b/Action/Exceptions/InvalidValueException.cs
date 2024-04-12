namespace Savor22b.Action.Exceptions;


[Serializable]
public class InvalidValueException : ActionException
{
    public InvalidValueException(string message, int? errorCode = null)
        : base(message, "InvalidValueException", errorCode)
    {
    }
}
