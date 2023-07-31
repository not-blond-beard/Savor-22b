namespace Savor22b.Action.Exceptions;


[Serializable]
public class HouseAlreadyPlacedException : ActionException
{
    public HouseAlreadyPlacedException(string message, int? errorCode = null)
        : base(message, "HouseAlreadyPlaced", errorCode)
    {
    }
}
