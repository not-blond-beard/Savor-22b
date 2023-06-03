using Bencodex.Types;
using Libplanet.Assets;

namespace Libplanet.Headless.Extensions;

public static class BencodexExtensions
{
    public static IValue Serialize(this Guid number) =>
        new Binary(number.ToByteArray());

    public static IValue ToBencodex(this FungibleAssetValue fav) =>
        new List(fav.Currency.Serialize(), (Integer)fav.RawValue);

    public static FungibleAssetValue ToFungibleAssetValue(this IValue value) =>
        FungibleAssetValue.FromRawValue(
            new Currency(((List)value)[0]),
            ((Integer)((List)value)[1]).Value
        );

    #region Address

    public static IValue ToBencodex(this Address address) => new Binary(address.ByteArray);
    public static Address ToAddress(this IValue value) => new(value);

    #endregion Address

    #region Guid

    public static Guid ToGuid(this IValue serialized) =>
        new Guid(((Binary)serialized).ToByteArray());

    #endregion Guid
}
