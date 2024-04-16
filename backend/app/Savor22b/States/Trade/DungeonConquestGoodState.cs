namespace Savor22b.States.Trade;

using System;
using Bencodex.Types;
using Libplanet.Assets;
using Libplanet;
using Libplanet.Headless.Extensions;

public class DungeonConquestGoodState : TradeGood
{
    public int DungeonId { get; private set; }

    public DungeonConquestGoodState(
        Address sellerAddress,
        Guid productId,
        FungibleAssetValue price,
        int dungeonId)
       : base(sellerAddress, productId, price, nameof(DungeonConquestGoodState))
    {
        DungeonId = dungeonId;
    }

    public DungeonConquestGoodState(Dictionary serialized)
        : base(serialized)
    {
        DungeonId = serialized[nameof(DungeonId)].ToInteger();
    }

    public override IValue Serialize()
    {
        var baseSerialized = base.Serialize() as Dictionary;
        baseSerialized = baseSerialized.Add((Text)nameof(DungeonId), DungeonId.Serialize());

        return baseSerialized;
    }
}
