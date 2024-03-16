namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Exceptions;
using Savor22b.Constants;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;

[ActionType(nameof(ListItemInTradeStoreAction))]
public class ListItemInTradeStoreAction : SVRAction
{
    public ListItemInTradeStoreAction() { }

    public ListItemInTradeStoreAction(Guid tradeItemStateId, RefrigeratorState food, FungibleAssetValue price)
    {
        TradeItemStateId = tradeItemStateId;
        Price = price;
        Food = food;
        Items = null;
    }

    public ListItemInTradeStoreAction(Guid tradeItemStateId, List<ItemState> items, FungibleAssetValue price)
    {
        TradeItemStateId = tradeItemStateId;
        Price = price;
        Food = null;
        Items = items.ToImmutableList();
    }

    public Guid TradeItemStateId;

    public FungibleAssetValue Price;

    public RefrigeratorState? Food;

    public ImmutableList<ItemState>? Items;

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(TradeItemStateId)] = TradeItemStateId.Serialize(),
            [nameof(Price)] = Price.ToBencodex(),
            [nameof(Food)] = Food is null ? Null.Value : Food.Serialize(),
            [nameof(Items)] = Items is null ? Null.Value : (Bencodex.Types.List)Items.Select(i => i.Serialize()),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        TradeItemStateId = plainValue[nameof(TradeItemStateId)].ToGuid();
        Price = plainValue[nameof(Price)].ToFungibleAssetValue();
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        IAccountStateDelta states = ctx.PreviousStates;

        RootState rootState = states.GetState(ctx.Signer) is Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();
        // var inventoryState = rootState.InventoryState;

        // var desiredEquipment = FindRandomSeedItem();
        // var itemState = new ItemState(ItemStateID, desiredEquipment.ID);

        // states = states.TransferAsset(
        //     ctx.Signer,
        //     Recipient,
        //     desiredEquipment.PriceToFungibleAssetValue(),
        //     allowNegativeBalance: false
        // );
        // inventoryState = inventoryState.AddItem(itemState);
        // rootState.SetInventoryState(inventoryState);

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
