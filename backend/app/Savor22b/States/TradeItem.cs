namespace Savor22b.States;

using System;
using Bencodex.Types;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;
using Libplanet;

[Serializable]
public class TradeItem
{
    public readonly Address SellerAddress;

    public readonly Guid ProductId;

    public readonly FungibleAssetValue Price;

    public readonly RefrigeratorState? Food;

    public readonly List<ItemState>? Items;

    public TradeItem(
        Address sellerAddress,
        Guid productId,
        FungibleAssetValue price,
        RefrigeratorState food)
    {
        SellerAddress = sellerAddress;
        ProductId = productId;
        Price = price;
        Food = food;
        Items = null;
    }

    public TradeItem(
        Address sellerAddress,
        Guid productId,
        FungibleAssetValue price,
        List<ItemState> items)
    {
        SellerAddress = sellerAddress;
        ProductId = productId;
        Price = price;
        Food = null;
        Items = items;
    }

    public TradeItem(Dictionary serialized)
    {
        SellerAddress = serialized[nameof(SellerAddress)].ToAddress();
        ProductId = serialized[nameof(ProductId)].ToGuid();
        Price = serialized[nameof(Price)].ToFungibleAssetValue();
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(SellerAddress), SellerAddress.ToBencodex()),
            new KeyValuePair<IKey, IValue>((Text)nameof(ProductId), ProductId.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(Price), Price.ToBencodex()),
            new KeyValuePair<IKey, IValue>((Text)nameof(Food), Food is null ? Null.Value : Food.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(Items), Items is null ? Null.Value : (Bencodex.Types.List)Items.Select(i => i.Serialize())),
        };
        return new Dictionary(pairs);
    }
}
