namespace Savor22b.Action.Exceptions;

[Serializable]
public class InvalidDungeonException : ActionException
{
    public InvalidDungeonException(string message, int? errorCode = null)
        : base(message, "InvalidDungeon", errorCode) { }
}
