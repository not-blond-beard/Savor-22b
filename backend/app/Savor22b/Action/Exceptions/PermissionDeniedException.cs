namespace Savor22b.Action.Exceptions;


[Serializable]
public class PermissionDeniedException : ActionException
{
    public PermissionDeniedException(string message, int? errorCode = null)
        : base(message, "PermissionDeniedException", errorCode)
    {
    }
}
