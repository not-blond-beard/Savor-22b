namespace Savor22b.States.Trade;

using System;
using Bencodex.Types;
using Libplanet.Assets;
using Libplanet;

public class FoodGoodState : TradeGood
{
    public RefrigeratorState Food { get; private set; }

    public FoodGoodState(
        Address sellerAddress,
        Guid productId,
        FungibleAssetValue price,
        RefrigeratorState food)
       : base(sellerAddress, productId, price, nameof(FoodGoodState))
    {
        Food = food;
    }

    public FoodGoodState(Dictionary serialized)
        : base(serialized)
    {
        Food = new RefrigeratorState((Dictionary)serialized[nameof(Food)]);
    }

    public IValue Serialize()
    {
        var baseSerialized = base.Serialize() as Dictionary;
        baseSerialized = baseSerialized.Add((Text)nameof(Food), Food.Serialize());

        return baseSerialized;
    }
}
