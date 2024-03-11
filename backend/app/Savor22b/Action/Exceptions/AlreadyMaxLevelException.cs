namespace Savor22b.Action.Exceptions;

[Serializable]
public class AlreadyMaxLevelException : ActionException
{
    public AlreadyMaxLevelException(string message, int? errorCode = null)
        : base(message, "AlreadyMaxLevelException", errorCode)
    {
    }
}
