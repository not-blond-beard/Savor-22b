namespace Savor22b.Action.Exceptions;


[Serializable]
public class HarvestableFieldException : ActionException
{
    public HarvestableFieldException(string message, int? errorCode = null)
        : base(message, "HarvestableField", errorCode)
    {
    }
}
