namespace Savor22b.Action.Exceptions;


[Serializable]
public class AlreadyUsingEquipmentException : ActionException
{
    public AlreadyUsingEquipmentException(string message, int? errorCode = null)
        : base(message, "AlreadyUsingEquipment", errorCode)
    {
    }
}
