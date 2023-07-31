namespace Savor22b.Action.Exceptions;

using System.Runtime.Serialization;


[Serializable]
public class NotEnoughRawMaterialsException : ActionException
{
    public NotEnoughRawMaterialsException(string message, int? errorCode = null)
        : base(message, "NotEnoughRawMaterials", errorCode)
    {
    }
}
