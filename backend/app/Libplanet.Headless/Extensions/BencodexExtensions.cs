using Bencodex.Types;
using Libplanet.Assets;
using System.Globalization;

namespace Libplanet.Headless.Extensions;

public static class BencodexExtensions
{

    public static IValue Serialize<T>(Func<T, IValue> serializer, T? value)
        where T : struct
    {
        return value is T v ? serializer(v) : Null.Value;
    }

    public static T? Deserialize<T>(Func<IValue, T> deserializer, IValue serialized)
        where T : struct
    {
        return serialized is Null ? (T?)null : deserializer(serialized);
    }

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

    public static IValue Serialize(this Guid number) =>
        new Binary(number.ToByteArray());

    public static IValue Serialize(this Guid? number) =>
        Serialize(Serialize, number);

    public static Guid ToGuid(this IValue serialized) =>
        new Guid(((Binary)serialized).ToByteArray());

    public static Guid? ToNullableGuid(this IValue serialized) =>
        Deserialize(ToGuid, serialized);

    #endregion Guid

    #region int

    public static IValue Serialize(this int number) =>
        (Text)number.ToString(CultureInfo.InvariantCulture);

    public static IValue Serialize(this int? number) =>
        Serialize(Serialize, number);

    public static int ToInteger(this IValue serialized) =>
        int.Parse(((Text)serialized).Value, CultureInfo.InvariantCulture);

    public static int? ToNullableInteger(this IValue serialized) =>
        Deserialize(ToInteger, serialized);

    #endregion int

    #region long

    public static IValue Serialize(this long number) =>
        (Text)number.ToString(CultureInfo.InvariantCulture);

    public static IValue Serialize(this long? number) =>
        Serialize(Serialize, number);

    public static long ToLong(this IValue serialized) =>
        long.Parse(((Text)serialized).Value, CultureInfo.InvariantCulture);

    public static long? ToNullableLong(this IValue serialized) =>
        Deserialize(ToLong, serialized);

    #endregion long

    #region Text

    public static IValue Serialize(this string text) =>
        (Text)text;

    public static string ToDotnetString(this IValue serialized) => ((Text) serialized).Value;

    #endregion Text
}
