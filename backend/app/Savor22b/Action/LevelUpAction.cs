namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Exceptions;
using Savor22b.Model;
using Savor22b.States;

[ActionType(nameof(LevelUpAction))]
public class LevelUpAction : SVRAction
{
    public LevelUpAction()
    {
    }

    public LevelUpAction(Guid foodStateID)
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

        if (!food.IsSuperFood)
        {
            throw new SuperFoodRequiredException($"Food `{FoodStateID}` is not Super Food.");
        }

        Level? nextLevelInfo = CsvDataHelper.GetLevelByID(food.Level + 1);

        if (nextLevelInfo == null)
        {
            throw new AlreadyMaxLevelException($"Already Max Level.");
        }

        var msgs = inventoryState.ItemStateList.FindAll((e) => e.ItemID == 3);
        if (msgs.Count < nextLevelInfo.RequiredMsgCount)
        {
            throw new NotHaveRequiredException("Not Enough Msg");
        }

        for (int i = 0; i < nextLevelInfo.RequiredMsgCount; i++)
        {
            inventoryState = inventoryState.RemoveItem(msgs[i].StateID);
        }

        food = food.LevelUp();

        inventoryState = inventoryState.RemoveRefrigeratorItem(food.StateID);
        inventoryState = inventoryState.AddRefrigeratorItem(food);
        rootState.SetInventoryState(inventoryState);

        return states.SetState(context.Signer, rootState.Serialize());
    }

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        FoodStateID = plainValue[nameof(FoodStateID)].ToGuid();
    }
}
