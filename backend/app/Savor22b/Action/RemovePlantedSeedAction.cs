namespace Savor22b.Action;

using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.State;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;
using Libplanet.Headless.Extensions;
using Savor22b.Action.Exceptions;
using Savor22b.Action.Util;

[ActionType(nameof(RemovePlantedSeedAction))]
public class RemovePlantedSeedAction : SVRAction
{
    public int RemoveFieldIndex;

    public RemovePlantedSeedAction() { }

    public RemovePlantedSeedAction(int removeFieldIndex)
    {
        RemoveFieldIndex = removeFieldIndex;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(RemoveFieldIndex)] = RemoveFieldIndex.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        RemoveFieldIndex = plainValue[nameof(RemoveFieldIndex)].ToInteger();
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        IAccountStateDelta states = ctx.PreviousStates;
        RootState rootState = states.GetState(ctx.Signer)
            is Bencodex.Types.Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();

        Validation.EnsureReplaceInProgress(rootState, ctx.BlockIndex);

        VillageState? villageState = rootState.VillageState;

        if (villageState is null)
        {
            throw new InvalidVillageStateException("VillageState is null");
        }

        HouseFieldState houseFieldState =
            villageState.HouseFieldStates[RemoveFieldIndex]
            ?? throw new InvalidFieldIndexException("FieldIndex is invalid");

        if (houseFieldState.IsHarvestable(ctx.BlockIndex))
        {
            throw new HarvestableFieldException("Field is harvestable");
        }

        villageState.RemoveHouseFieldState(RemoveFieldIndex);
        rootState.SetVillageState(villageState);

        states = states.SetState(ctx.Signer, rootState.Serialize());

        return states;
    }
}
