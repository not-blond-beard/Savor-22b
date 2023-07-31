namespace Savor22b.Action.Exceptions;


[Serializable]
public class InvalidVillageException : ActionException
{
    public InvalidVillageException(string message, int? errorCode = null)
        : base(message, "InvalidVillage", errorCode)
    {
    }
}
