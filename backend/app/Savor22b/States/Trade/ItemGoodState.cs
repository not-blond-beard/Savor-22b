namespace Savor22b.States.Trade;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Assets;
using Libplanet;

public class ItemGood : TradeGood
{
    public ImmutableList<ItemState> Items { get; private set; }

    public ItemGood(
        Address sellerAddress,
        Guid productId,
        FungibleAssetValue price,
        ImmutableList<ItemState> items)
       : base(sellerAddress, productId, price, nameof(FoodGoodState))
    {
        Items = items;
    }

    public ItemGood(Dictionary serialized)
        : base(serialized)
    {
        Items = ((List)serialized["Items"]).Select(dict => new ItemState((Dictionary)dict)).ToImmutableList();
    }

    public IValue Serialize()
    {
        var baseSerialized = base.Serialize() as Dictionary;
        var itemsSerialized = Items.Select(item => item.Serialize()).ToList();

        baseSerialized = baseSerialized.Add((Text)nameof(Items), (List)Items.Select(i => i.Serialize()));
        return baseSerialized;
    }
}
