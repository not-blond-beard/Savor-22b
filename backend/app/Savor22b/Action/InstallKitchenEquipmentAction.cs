namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Util;
using Savor22b.Action.Exceptions;
using Savor22b.States;

[ActionType(nameof(InstallKitchenEquipmentAction))]
public class InstallKitchenEquipmentAction : SVRAction
{
    public Guid KitchenEquipmentStateIDToUse;
    public int SpaceNumber;

    public InstallKitchenEquipmentAction() { }

    public InstallKitchenEquipmentAction(Guid kitchenEquipmentStateID, int spaceNumber)
    {
        KitchenEquipmentStateIDToUse = kitchenEquipmentStateID;
        SpaceNumber = spaceNumber;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(KitchenEquipmentStateIDToUse)] = KitchenEquipmentStateIDToUse.Serialize(),
            [nameof(SpaceNumber)] = SpaceNumber.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        KitchenEquipmentStateIDToUse = plainValue[nameof(KitchenEquipmentStateIDToUse)].ToGuid();
        SpaceNumber = plainValue[nameof(SpaceNumber)].ToInteger();
    }

    private KitchenEquipmentState GetKitchenEquipmentState(
        RootState rootState,
        Guid kitchenEquipmentStateIDToUse
    )
    {
        var kitchenEquipmentState = rootState.InventoryState.GetKitchenEquipmentState(
            kitchenEquipmentStateIDToUse
        );

        if (kitchenEquipmentState is null)
        {
            throw new NotHaveRequiredException(
                $"You don't have `{kitchenEquipmentStateIDToUse}` kitchen equipment state"
            );
        }

        return kitchenEquipmentState;
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

        Validation.EnsureReplaceInProgress(rootState, ctx.BlockIndex);
        Validation.EnsureVillageStateExists(rootState);

        var kitchenEquipmentState = GetKitchenEquipmentState(
            rootState,
            KitchenEquipmentStateIDToUse
        );
        rootState.VillageState?.HouseState.KitchenState.InstallKitchenEquipment(
            kitchenEquipmentState,
            SpaceNumber
        );

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
