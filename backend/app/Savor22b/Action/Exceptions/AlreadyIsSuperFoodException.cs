namespace Savor22b.Action.Exceptions;

[Serializable]
public class AlreadyIsSuperFoodException : ActionException
{
    public AlreadyIsSuperFoodException(string message, int? errorCode = null)
        : base(message, "AlreadyIsSuperFood", errorCode)
    {
    }
}
