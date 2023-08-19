namespace Savor22b.Action.Exceptions;


[Serializable]
public class NotHaveRequiredEquipmentException : ActionException
{
    public NotHaveRequiredEquipmentException(string message, int? errorCode = null)
        : base(message, "NotHaveRequiredEquipment", errorCode)
    {
    }
}
