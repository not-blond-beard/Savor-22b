namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Exceptions;
using Savor22b.States;
using Savor22b.Constants;

[ActionType(nameof(UseLifeStoneAction))]
public class UseLifeStoneAction : SVRAction
{
    public UseLifeStoneAction()
    {
    }

    public UseLifeStoneAction(Guid foodStateID)
    {
        FoodStateID = foodStateID;
    }

    public Guid FoodStateID { get; private set; }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(FoodStateID)] = FoodStateID.Serialize(),
        }.ToImmutableDictionary();

    public override IAccountStateDelta Execute(IActionContext context)
    {
        if (context.Rehearsal)
        {
            return context.PreviousStates;
        }

        IAccountStateDelta states = context.PreviousStates;

        RootState rootState = states.GetState(context.Signer) is Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();

        InventoryState inventoryState = rootState.InventoryState;

        var food = inventoryState.GetRefrigeratorItem(FoodStateID);

        if (food is null)
        {
            throw new NotFoundDataException($"NotFound `{FoodStateID}` food state id");
        }

        if (food.GetEdibleType() != Edible.FOOD)
        {
            throw new NotHaveRequiredException($"`{FoodStateID}` is not Food.");
        }

        if (food.IsSuperFood)
        {
            throw new AlreadyIsSuperFoodException($"Food `{FoodStateID}` is already Super Food.");
        }

        var singleLifeStone = inventoryState.ItemStateList.FirstOrDefault((e) => e.ItemID == 2);
        if (singleLifeStone is null)
        {
            throw new NotHaveRequiredException("No LifeStone is found in user's inventory.");
        }

        food.IsSuperFood = true;

        inventoryState = inventoryState.RemoveRefrigeratorItem(food.StateID);
        inventoryState = inventoryState.AddRefrigeratorItem(food);
        inventoryState = inventoryState.RemoveItem(singleLifeStone.StateID);
        rootState.SetInventoryState(inventoryState);
        return states.SetState(context.Signer, rootState.Serialize());
    }

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        FoodStateID = plainValue[nameof(FoodStateID)].ToGuid();
    }
}
