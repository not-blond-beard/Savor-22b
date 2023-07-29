namespace Savor22b.Action;

using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.State;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;
using Libplanet.Headless.Extensions;


[ActionType(nameof(UseRandomSeedItemAction))]
public class UseRandomSeedItemAction : SVRAction
{
    public Guid SeedStateID;

    public UseRandomSeedItemAction()
    {
    }

    public UseRandomSeedItemAction(Guid seedStateID)
    {
        SeedStateID = seedStateID;
    }


    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>(){
            [nameof(SeedStateID)] = SeedStateID.Serialize()
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        SeedStateID = plainValue[nameof(SeedStateID)].ToGuid();
    }

    private SeedState generateRandomSeed(IRandom random)
    {
        CsvParser<Seed> csvParser = new CsvParser<Seed>();

        var csvPath = Paths.GetCSVDataPath("seed.csv");

        var seeds = csvParser.ParseCsv(csvPath);
        int randomIndex = random.Next(0, seeds.Count);

        var randomSeedData = seeds[randomIndex];
        var randomSeed = new SeedState(SeedStateID, randomSeedData.Id);

        return randomSeed;
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        IAccountStateDelta states = ctx.PreviousStates;

        InventoryState inventoryState =
            states.GetState(ctx.Signer) is Bencodex.Types.Dictionary stateEncoded
                ? new InventoryState(stateEncoded)
                : new InventoryState();

        SeedState seedState = generateRandomSeed(ctx.Random);
        inventoryState = inventoryState.AddSeed(seedState);

        var encodedValue = inventoryState.Serialize();
        var statesWithUpdated = states.SetState(ctx.Signer, encodedValue);

        return statesWithUpdated;
    }
}
