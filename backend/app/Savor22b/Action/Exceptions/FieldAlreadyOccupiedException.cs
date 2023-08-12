namespace Savor22b.Action.Exceptions;


[Serializable]
public class FieldAlreadyOccupiedException : ActionException
{
    public FieldAlreadyOccupiedException(string message, int? errorCode = null)
        : base(message, "FieldAlreadyOccupied", errorCode)
    {
    }
}
