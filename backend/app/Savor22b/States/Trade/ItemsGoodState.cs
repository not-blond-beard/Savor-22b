namespace Savor22b.States.Trade;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Assets;
using Libplanet;

public class ItemsGoodState : TradeGood
{
    public ImmutableList<ItemState> Items { get; private set; }

    public ItemsGoodState(
        Address sellerAddress,
        Guid productId,
        FungibleAssetValue price,
        ImmutableList<ItemState> items)
       : base(sellerAddress, productId, price, nameof(ItemsGoodState))
    {
        Items = items;
    }

    public ItemsGoodState(Dictionary serialized)
        : base(serialized)
    {
        Items = ((List)serialized[nameof(Items)]).Select(dict => new ItemState((Dictionary)dict)).ToImmutableList();
    }

    public override IValue Serialize()
    {
        var baseSerialized = base.Serialize() as Dictionary;

        baseSerialized = baseSerialized.Add((Text)nameof(Items), new List(Items.Select(item => item.Serialize())));
        return baseSerialized;
    }
}
