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

[ActionType(nameof(PlantingSeedAction))]
public class PlantingSeedAction : SVRAction
{
    public Guid SeedGuid;
    public int FieldIndex;
    public Guid ItemStateIdToUse;

    public PlantingSeedAction() { }

    public PlantingSeedAction(Guid seedGuid, int fieldIndex, Guid itemStateIdToUse)
    {
        SeedGuid = seedGuid;
        FieldIndex = fieldIndex;
        ItemStateIdToUse = itemStateIdToUse;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(SeedGuid)] = SeedGuid.Serialize(),
            [nameof(FieldIndex)] = FieldIndex.Serialize(),
            [nameof(ItemStateIdToUse)] = ItemStateIdToUse.Serialize()
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        SeedGuid = plainValue[nameof(SeedGuid)].ToGuid();
        FieldIndex = plainValue[nameof(FieldIndex)].ToInteger();
        ItemStateIdToUse = plainValue[nameof(ItemStateIdToUse)].ToGuid();
    }

    public void checkAndRaisePlantingAble(RootState rootState)
    {
        if (rootState.VillageState is null)
        {
            throw new InvalidVillageStateException("VillageState is null");
        }

        if (FieldIndex < 0 || FieldIndex >= VillageState.HouseFieldCount)
        {
            throw new InvalidFieldIndexException("FieldIndex is invalid");
        }

        if (rootState.VillageState.HouseFieldStates[FieldIndex] is not null)
        {
            throw new FieldAlreadyOccupiedException("Field is already occupied");
        }
    }

    private Seed generateRandomSeed(IRandom random, int villageId)
    {
        int randomIndex;
        var seeds = CsvDataHelper.GetSeedCSVData();
        var village = CsvDataHelper.GetVillageCharacteristicByVillageId(villageId)!;

        do
        {
            randomIndex = random.Next(0, seeds.Count);
        } while (!village.AvailableSeedIdList.Contains(seeds[randomIndex].Id));

        var randomSeedCsvData = seeds[randomIndex];

        return randomSeedCsvData;
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

        Validation.EnsureReplaceInProgress(rootState, ctx.BlockIndex);

        checkAndRaisePlantingAble(rootState);

        InventoryState inventoryState = rootState.InventoryState;
        inventoryState = FindAndRemoveItem(inventoryState);

        Seed seedCsvData = generateRandomSeed(
            ctx.Random,
            rootState.VillageState!.HouseState.VillageID
        );
        var seedState = new SeedState(SeedGuid, seedCsvData.Id);
        inventoryState = inventoryState.AddSeed(seedState);

        rootState.SetInventoryState(inventoryState);

        HouseFieldState houseFieldState = new HouseFieldState(
            SeedGuid,
            seedCsvData.Id,
            ctx.BlockIndex,
            seedCsvData.RequiredBlock
        );

        rootState.VillageState!.UpdateHouseFieldState(FieldIndex, houseFieldState);

        states = states.SetState(ctx.Signer, rootState.Serialize());

        return states;
    }
}
