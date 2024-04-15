namespace Savor22b.Action;

using Libplanet.Action;
using Savor22b.States;
using Savor22b.Action.Util;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Headless.Extensions;
using Libplanet.State;

[ActionType(nameof(RemoveInstalledKitchenEquipmentAction))]
public class RemoveInstalledKitchenEquipmentAction : SVRAction
{
    public RemoveInstalledKitchenEquipmentAction() { }

    public RemoveInstalledKitchenEquipmentAction(Guid installedEquipmentStateId)
    {
        InstalledEquipmentStateId = installedEquipmentStateId;
    }

    public Guid InstalledEquipmentStateId { get; private set; }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(InstalledEquipmentStateId)] = InstalledEquipmentStateId.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        InstalledEquipmentStateId = plainValue[nameof(InstalledEquipmentStateId)].ToGuid();
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

        KitchenState kitchenState = rootState.VillageState!.HouseState.KitchenState;

        kitchenState.RemoveInstalledEquipment(InstalledEquipmentStateId);

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
