namespace Savor22b.Extensions;

using Bencodex.Types;


public static class BencodexExtensions
{
    public static IValue Serialize(this Guid number) =>
        new Binary(number.ToByteArray());

    #region Guid

    public static Guid ToGuid(this IValue serialized) =>
        new Guid(((Binary)serialized).ToByteArray());

    #endregion Guid
}
