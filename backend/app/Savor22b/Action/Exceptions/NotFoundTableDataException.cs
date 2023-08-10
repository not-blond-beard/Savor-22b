namespace Savor22b.Action.Exceptions;


[Serializable]
public class NotFoundTableDataException : ActionException
{
    public NotFoundTableDataException(string message, int? errorCode = null)
        : base(message, "NotFoundTableData", errorCode)
    {
    }
}
