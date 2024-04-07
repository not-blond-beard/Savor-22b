namespace Savor22b.States.Trade;

using System;
using Bencodex.Types;
using Libplanet;
using Savor22b.Constants;

public class TradeInventoryState : State
{
    public static readonly Address StateAddress = Addresses.TradeStoreAddress;

    public TradeInventoryState(Dictionary encoded)
    {
        TradeGoods = ((List)encoded[nameof(TradeGoods)])
            .Select(item => TradeGoodFactory.CreateInstance((Dictionary)item))
            .ToDictionary(good => good.ProductStateId, good => good);
    }

    public Dictionary<Guid, TradeGood> TradeGoods { get; private set; }

    public IValue Serialize()
    {
        var tradeGoodsPairs = TradeGoods.Values
            .Select(good => new KeyValuePair<IKey, IValue>(
                (Text)good.ProductStateId.ToString(), good.Serialize()));

        var tradeGoodsDict = new Dictionary(tradeGoodsPairs);

        return new Dictionary(new[] {
            new KeyValuePair<IKey, IValue>((Text)nameof(TradeGoods), tradeGoodsDict)
        });
    }
}
