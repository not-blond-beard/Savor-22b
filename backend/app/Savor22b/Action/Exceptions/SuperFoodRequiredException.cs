namespace Savor22b.Action.Exceptions;

[Serializable]
public class SuperFoodRequiredException : ActionException
{
    public SuperFoodRequiredException(string message, int? errorCode = null)
        : base(message, "SuperFoodRequiredException", errorCode)
    {
    }
}
