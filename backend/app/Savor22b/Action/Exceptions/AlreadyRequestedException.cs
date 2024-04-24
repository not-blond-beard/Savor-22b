namespace Savor22b.Action.Exceptions;

[Serializable]
public class AlreadyRequestedException : ActionException
{
    public AlreadyRequestedException(string message, int? errorCode = null)
        : base(message, "AlreadyRequested", errorCode) { }
}
