namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Exceptions;
using Savor22b.Action.Util;
using Savor22b.States;

[ActionType(nameof(CancelFoodAction))]
public class CancelFoodAction : SVRAction
{
    public Guid FoodStateID;

    public CancelFoodAction() { }

    public CancelFoodAction(Guid foodStateID)
    {
        FoodStateID = foodStateID;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(FoodStateID)] = FoodStateID.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        FoodStateID = plainValue[nameof(FoodStateID)].ToGuid();
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

        Validation.EnsureVillageStateExists(rootState);

        InventoryState inventoryState = rootState.InventoryState;
        KitchenState kitchenState = rootState.VillageState!.HouseState.KitchenState;

        var food = inventoryState.GetRefrigeratorItem(FoodStateID);

        if (food is null)
        {
            throw new NotFoundDataException($"NotFound `{FoodStateID}` food state id");
        }

        if (food!.AvailableBlockIndex < ctx.BlockIndex)
        {
            throw new NotCookingException("This food is available");
        }

        foreach (var kitchenEquipmentStateId in food!.UsedKitchenEquipmentStateIds)
        {
            var kitchenEquipmentState = inventoryState.GetKitchenEquipmentState(
                kitchenEquipmentStateId
            );

            if (kitchenEquipmentState is null)
            {
                throw new NotFoundDataException(
                    $"NotFound `{kitchenEquipmentStateId}` kitchen equipment state id"
                );
            }

            var kitchenEquipmentCategory = CsvDataHelper.GetKitchenEquipmentCategoryByID(
                kitchenEquipmentState.KitchenEquipmentCategoryID
            );

            if (kitchenEquipmentCategory is null)
            {
                throw new NotFoundTableDataException("Not found category table");
            }

            if (kitchenEquipmentCategory!.Category == "main")
            {
                for (var i = 1; i < 4; i++)
                {
                    if (
                        kitchenState
                            .GetApplianceSpaceStateByNumber(i)
                            .InstalledKitchenEquipmentStateId == kitchenEquipmentStateId
                    )
                    {
                        kitchenState.GetApplianceSpaceStateByNumber(i).StopCooking();
                    }
                }
            }
            else
            {
                kitchenEquipmentState = kitchenEquipmentState.StopCooking();
                inventoryState = inventoryState.RemoveKitchenEquipmentItem(
                    kitchenEquipmentState.StateID
                );
                inventoryState = inventoryState.AddKitchenEquipmentItem(kitchenEquipmentState);
            }
        }
        inventoryState = inventoryState.RemoveRefrigeratorItem(food.StateID);
        rootState.SetInventoryState(inventoryState);

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
