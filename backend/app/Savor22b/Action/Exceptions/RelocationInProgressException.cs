namespace Savor22b.Action.Exceptions;

[Serializable]
public class RelocationInProgressException : ActionException
{
    public RelocationInProgressException(string message, int? errorCode = null)
        : base(message, "RelocationInProgress", errorCode) { }
}
