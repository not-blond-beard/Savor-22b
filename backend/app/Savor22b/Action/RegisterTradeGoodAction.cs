namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.States;
using Savor22b.States.Trade;

[ActionType(nameof(RegisterTradeGoodAction))]
public class RegisterTradeGoodAction : SVRAction
{
    public RegisterTradeGoodAction() { }

    public RegisterTradeGoodAction(string goodType, FungibleAssetValue price, RefrigeratorState foodState)
    {
        GoodType = goodType;
        Price = price;
        FoodState = foodState;
        ItemStates = null;
    }

    public string GoodType;

    public FungibleAssetValue Price;

    public RefrigeratorState? FoodState;

    public ImmutableList<ItemState>? ItemStates;

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(GoodType)] = GoodType.Serialize(),
            [nameof(Price)] = Price.ToBencodex(),
            [nameof(FoodState)] = FoodState is null ? Null.Value : FoodState.Serialize(),
            [nameof(ItemStates)] = ItemStates is null ? Null.Value : (Bencodex.Types.List)ItemStates.Select(i => i.Serialize()),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        GoodType = plainValue[nameof(GoodType)].ToString();
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
