namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Exceptions;
using Savor22b.States;
using Savor22b.States.Trade;

[ActionType(nameof(RegisterTradeGoodAction))]
public class RegisterTradeGoodAction : SVRAction
{
    public RegisterTradeGoodAction() { }

    public RegisterTradeGoodAction(FungibleAssetValue price, Guid foodStateId)
    {
        Price = price;
        FoodStateId = foodStateId;
        ItemStateIds = null;
        DungeonId = null;
    }

    public RegisterTradeGoodAction(FungibleAssetValue price, ImmutableList<Guid> itemStateIds)
    {
        Price = price;
        FoodStateId = null;
        ItemStateIds = itemStateIds;
        DungeonId = null;
    }

    public RegisterTradeGoodAction(FungibleAssetValue price, int dungeonId)
    {
        Price = price;
        FoodStateId = null;
        ItemStateIds = null;
        DungeonId = dungeonId;
    }

    public FungibleAssetValue Price;

    public Guid? FoodStateId;

    public int? DungeonId;

    public ImmutableList<Guid>? ItemStateIds;

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(Price)] = Price.ToBencodex(),
            [nameof(FoodStateId)] = FoodStateId.Serialize(),
            [nameof(ItemStateIds)] = ItemStateIds is null
                ? Null.Value
                : (List)ItemStateIds.Select(i => i.Serialize()),
            [nameof(DungeonId)] = DungeonId.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        Price = plainValue[nameof(Price)].ToFungibleAssetValue();
        FoodStateId = plainValue[nameof(FoodStateId)].ToGuid();
        ItemStateIds =
            plainValue[nameof(ItemStateIds)] is Null
                ? null
                : ((List)plainValue[nameof(ItemStateIds)])
                    .Select(e => e.ToGuid())
                    .ToImmutableList();
        DungeonId = plainValue[nameof(DungeonId)].ToNullableInteger();
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
        TradeInventoryState tradeInventoryState = states.GetState(TradeInventoryState.StateAddress)
            is Dictionary tradeInventoryStateEncoded
            ? new TradeInventoryState(tradeInventoryStateEncoded)
            : new TradeInventoryState();
        GlobalDungeonState globalDungeonState = states.GetState(GlobalDungeonState.StateAddress)
            is Dictionary globalDungeonStateEncoded
            ? new GlobalDungeonState(globalDungeonStateEncoded)
            : new GlobalDungeonState();

        var inventoryState = rootState.InventoryState;

        if (FoodStateId is not null)
        {
            var foodState = inventoryState.GetRefrigeratorItem(FoodStateId.Value);
            var foodGoodState = new FoodGoodState(
                ctx.Signer,
                ctx.Random.GenerateRandomGuid(),
                Price,
                foodState
            );
            inventoryState = inventoryState.RemoveRefrigeratorItem(FoodStateId.Value);
            tradeInventoryState = tradeInventoryState.RegisterGood(foodGoodState);
        }
        else if (ItemStateIds is not null)
        {
            var itemStates = new List<ItemState>();
            foreach (var itemStateId in ItemStateIds)
            {
                itemStates.Add(inventoryState.GetItem(itemStateId));
                inventoryState = inventoryState.RemoveItem(itemStateId);
            }

            var itemsGoodState = new ItemsGoodState(
                ctx.Signer,
                ctx.Random.GenerateRandomGuid(),
                Price,
                itemStates.ToImmutableList()
            );

            tradeInventoryState = tradeInventoryState.RegisterGood(itemsGoodState);
        }
        else if (DungeonId is not null)
        {
            var dungeonConquestGoodState = new DungeonConquestGoodState(
                ctx.Signer,
                ctx.Random.GenerateRandomGuid(),
                Price,
                DungeonId.Value
            );

            if (!globalDungeonState.IsDungeonConquestAddress(DungeonId.Value, ctx.Signer))
            {
                throw new PermissionDeniedException("Permission denied");
            }

            tradeInventoryState = tradeInventoryState.RegisterGood(dungeonConquestGoodState);
        }
        else
        {
            throw new InvalidValueException($"ItemStateIds or FoodStateId required");
        }

        rootState.SetInventoryState(inventoryState);

        states = states.SetState(GlobalDungeonState.StateAddress, globalDungeonState.Serialize());
        states = states.SetState(TradeInventoryState.StateAddress, tradeInventoryState.Serialize());
        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
