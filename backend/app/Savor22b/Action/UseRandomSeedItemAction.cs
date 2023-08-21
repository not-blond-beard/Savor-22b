namespace Savor22b.Action;

using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Exceptions;
using Savor22b.States;
using Savor22b.Action.Util;

[ActionType(nameof(UseRandomSeedItemAction))]
public class UseRandomSeedItemAction : SVRAction
{
    public Guid SeedStateID;
    public Guid ItemStateIdToUse;

    public UseRandomSeedItemAction() { }

    public UseRandomSeedItemAction(Guid seedStateID, Guid itemStateIdToUse)
    {
        SeedStateID = seedStateID;
        ItemStateIdToUse = itemStateIdToUse;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(SeedStateID)] = SeedStateID.Serialize(),
            [nameof(ItemStateIdToUse)] = ItemStateIdToUse.Serialize()
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        SeedStateID = plainValue[nameof(SeedStateID)].ToGuid();
        ItemStateIdToUse = plainValue[nameof(ItemStateIdToUse)].ToGuid();
    }

    private SeedState generateRandomSeed(IRandom random, int villageId)
    {
        int randomIndex;
        var seeds = CsvDataHelper.GetSeedCSVData();
        var village = CsvDataHelper.GetVillageCharacteristicByVillageId(villageId)!;

        do
        {
            randomIndex = random.Next(0, seeds.Count);
        } while (!village.AvailableSeedIdList.Contains(seeds[randomIndex].Id));

        var randomSeedData = seeds[randomIndex];
        var randomSeed = new SeedState(SeedStateID, randomSeedData.Id);

        return randomSeed;
    }

    private InventoryState FindAndRemoveItem(InventoryState state)
    {
        var item = state.ItemStateList.Find(state => state.StateID == ItemStateIdToUse);

        if (item is null)
        {
            throw new NotHaveRequiredException($"You don't have `{ItemStateIdToUse}` item");
        }

        state = state.RemoveItem(ItemStateIdToUse);

        return state;
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        IAccountStateDelta states = ctx.PreviousStates;

        RootState rootState = states.GetState(ctx.Signer)
            is Bencodex.Types.Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();

        Validation.EnsureVillageStateExists(rootState);

        InventoryState inventoryState = rootState.InventoryState;

        SeedState seedState = generateRandomSeed(
            ctx.Random,
            rootState.VillageState!.HouseState.VillageID
        );
        inventoryState = FindAndRemoveItem(inventoryState);
        inventoryState = inventoryState.AddSeed(seedState);

        rootState.SetInventoryState(inventoryState);

        var encodedValue = rootState.Serialize();
        var statesWithUpdated = states.SetState(ctx.Signer, encodedValue);

        return statesWithUpdated;
    }
}
