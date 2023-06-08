namespace Savor22b.Action;

using System.Runtime.Serialization;


[Serializable]
public class NotEnoughRawMaterialsException : Exception, ISerializable
{
    public NotEnoughRawMaterialsException()
    {
    }

    public NotEnoughRawMaterialsException(string? message)
        : base(message)
    {
    }

    public NotEnoughRawMaterialsException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
