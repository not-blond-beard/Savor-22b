namespace Savor22b.States.Trade;

using System;
using Bencodex.Types;
using Libplanet;
using Libplanet.Headless.Extensions;
using Savor22b.Constants;

public class TradeInventoryState : State
{
    public static readonly Address StateAddress = Addresses.TradeStoreAddress;

    public TradeInventoryState()
    {
        TradeGoods = new Dictionary<Guid, TradeGood>();
    }

    public TradeInventoryState(Dictionary encoded)
    {
        TradeGoods = ((Dictionary)encoded[nameof(TradeGoods)]).ToDictionary(
            pair => pair.Key.ToGuid(),
            pair => TradeGoodFactory.CreateInstance((Dictionary)pair.Value)
        );
    }

    public Dictionary<Guid, TradeGood> TradeGoods { get; private set; }

    public IValue Serialize()
    {
        var tradeGoodsPairs = TradeGoods.Values
            .Select(good => new KeyValuePair<IKey, IValue>(
                (Binary)good.ProductStateId.Serialize(), good.Serialize()));

        var tradeGoodsDict = new Dictionary(tradeGoodsPairs);

        return new Dictionary(new[] {
            new KeyValuePair<IKey, IValue>((Text)nameof(TradeGoods), tradeGoodsDict)
        });
    }

    public TradeInventoryState RegisterGood(TradeGood good)
    {
        TradeGoods.Add(good.ProductStateId, good);
        return new TradeInventoryState((Dictionary)Serialize());
    }
}
