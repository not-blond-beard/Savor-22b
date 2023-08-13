namespace Savor22b.Action.Exceptions;


[Serializable]
public class WeedRemovalAbleException : ActionException
{
    public WeedRemovalAbleException(string message, int? errorCode = null)
        : base(message, "WeedRemovalAble", errorCode)
    {
    }
}
