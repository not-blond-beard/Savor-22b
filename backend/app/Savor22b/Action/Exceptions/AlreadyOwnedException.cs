namespace Savor22b.Action.Exceptions;

[Serializable]
public class AlreadyOwnedException : ActionException
{
    public AlreadyOwnedException(string message, int? errorCode = null)
        : base(message, "AlreadyOwned", errorCode) { }
}
