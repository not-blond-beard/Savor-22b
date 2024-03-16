namespace Savor22b.States;

using System;
using Bencodex.Types;
using Libplanet.Headless.Extensions;
using Libplanet;
using Savor22b.Constants;

public class TradeStoreState : State
{
    public static readonly Address StateAddress = Addresses.TradeStoreAddress;

    public TradeStoreState(Dictionary encoded)
    {
        TradItems = ((List)encoded[nameof(TradItems)]).Select(item => new TradeItem((Dictionary)item)).ToDictionary(i => i.ProductId, i => i);
    }

    public Dictionary<Guid, TradeItem> TradItems { get; private set; }

    public IValue Serialize()
    {
        var tradItemsPairs = TradItems.Select(item =>
            new KeyValuePair<IKey, IValue>(
                (Text)item.Key.Serialize(),
                item.Value.Serialize()
            )
        );

        var tradItemsDict = new Dictionary(tradItemsPairs);

        var statePairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(TradItems), tradItemsDict),
        };

        return new Dictionary(statePairs);
    }
}
