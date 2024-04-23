namespace Savor22b.Action;

using Libplanet.Action;
using Savor22b.States;
using Savor22b.Action.Util;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Headless.Extensions;
using Libplanet.State;

[ActionType(nameof(UninstallKitchenEquipmentAction))]
public class UninstallKitchenEquipmentAction : SVRAction
{
    public UninstallKitchenEquipmentAction() { }

    public UninstallKitchenEquipmentAction(int spaceNumber)
    {
        SpaceNumber = spaceNumber;
    }

    public int SpaceNumber { get; private set; }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(SpaceNumber)] = SpaceNumber.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        SpaceNumber = plainValue[nameof(SpaceNumber)].ToInteger();
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

        kitchenState.UninstalledEquipment(SpaceNumber);

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
