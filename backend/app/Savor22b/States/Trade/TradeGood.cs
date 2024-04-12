namespace Savor22b.States.Trade;

using System;
using Bencodex.Types;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;
using Libplanet;

public abstract class TradeGood
{
    public Address SellerAddress { get; private set; }

    public Guid ProductStateId { get; private set; }

    public FungibleAssetValue Price { get; private set; }

    public string Type { get; private set; }

    public TradeGood(
        Address sellerAddress,
        Guid productStateId,
        FungibleAssetValue price,
        string type)
    {
        SellerAddress = sellerAddress;
        ProductStateId = productStateId;
        Price = price;
        Type = type;
    }

    public TradeGood(Dictionary serialized)
    {
        SellerAddress = serialized[nameof(SellerAddress)].ToAddress();
        ProductStateId = serialized[nameof(ProductStateId)].ToGuid();
        Price = serialized[nameof(Price)].ToFungibleAssetValue();
        Type = serialized[nameof(Type)].ToString();
    }

    public virtual IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(SellerAddress), SellerAddress.ToBencodex()),
            new KeyValuePair<IKey, IValue>((Text)nameof(ProductStateId), ProductStateId.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(Price), Price.ToBencodex()),
            new KeyValuePair<IKey, IValue>((Text)nameof(Type), Type.Serialize()),
        };
        return new Dictionary(pairs);
    }
}
