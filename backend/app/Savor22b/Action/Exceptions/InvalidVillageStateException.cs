namespace Savor22b.Action.Exceptions;


[Serializable]
public class InvalidVillageStateException : ActionException
{
    public InvalidVillageStateException(string message, int? errorCode = null)
        : base(message, "InvalidVillageState", errorCode)
    {
    }
}
