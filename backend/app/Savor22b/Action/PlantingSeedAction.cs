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

[ActionType(nameof(PlantingSeedAction))]
public class PlantingSeedAction : SVRAction
{
    public Guid SeedGuid;
    public int FieldIndex;

    public PlantingSeedAction()
    {
    }

    public PlantingSeedAction(Guid seedGuid, int fieldIndex)
    {
        SeedGuid = seedGuid;
        FieldIndex = fieldIndex;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(SeedGuid)] = SeedGuid.Serialize(),
            [nameof(FieldIndex)] = FieldIndex.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        SeedGuid = plainValue[nameof(SeedGuid)].ToGuid();
        FieldIndex = plainValue[nameof(FieldIndex)].ToInteger();
    }

    public void checkAndRaisePlantingAble(
        RootState rootState
    )
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

        SeedState? seedState = rootState.InventoryState.GetSeedState(SeedGuid);

        if (seedState is null)
        {
            throw new NotFoundDataException("Seed not found");
        }
    }

    private Seed? getMatchedSeed(int seedId)
    {
        CsvParser<Seed> csvParser = new CsvParser<Seed>();

        var csvPath = Paths.GetCSVDataPath("seed.csv");

        var seeds = csvParser.ParseCsv(csvPath);

        var matchedSeed = seeds.Find(seed => seed.Id == seedId);

        return matchedSeed;
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        IAccountStateDelta states = ctx.PreviousStates;
        RootState rootState = states.GetState(ctx.Signer) is Bencodex.Types.Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();

        checkAndRaisePlantingAble(rootState);

        InventoryState inventoryState = rootState.InventoryState;
        SeedState seedState = rootState.InventoryState.GetSeedState(SeedGuid)!;

        inventoryState = inventoryState.RemoveSeed(SeedGuid);
        rootState.SetInventoryState(inventoryState);

        Seed seed = getMatchedSeed(seedState.SeedID)!;

        HouseFieldState houseFieldState = new HouseFieldState(
            SeedGuid,
            seedState.SeedID,
            ctx.BlockIndex,
            seed.RequiredBlock,
            null
        );

        rootState.VillageState!.UpdateHouseFieldState(
            FieldIndex,
            houseFieldState
        );

        states = states.SetState(ctx.Signer, rootState.Serialize());

        return states;
    }

}
