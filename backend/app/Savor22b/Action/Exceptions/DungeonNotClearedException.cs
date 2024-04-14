namespace Savor22b.Action.Exceptions;

[Serializable]
public class DungeonNotClearedException : ActionException
{
    public DungeonNotClearedException(string message, int? errorCode = null)
        : base(message, "DungeonNotCleared", errorCode) { }
}
