namespace Savor22b.Action.Exceptions;

[Serializable]
public class ResourceIsNotReadyException : ActionException
{
    public ResourceIsNotReadyException(string message, int? errorCode = null)
        : base(message, "ResourceIsNotReady", errorCode) { }
}
