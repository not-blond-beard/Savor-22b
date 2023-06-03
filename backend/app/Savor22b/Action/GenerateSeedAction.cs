namespace Savor22b.Action;

using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.State;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;


[ActionType("generate_seed")]
public class GenerateSeedAction : SVRAction
{
    public GenerateSeedAction()
    {
    }
    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>(){}.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
    }

    private SeedState generateRandomSeed(IRandom random, int newSeedId)
    {
        CsvParser<Seed> csvParser = new CsvParser<Seed>();

        var csvPath = Paths.GetCSVDataPath("seed.csv");

        var seeds = csvParser.ParseCsv(csvPath);
        int randomIndex = random.Next(0, seeds.Count);

        var randomSeedData = seeds[randomIndex];
        var randomSeed = new SeedState(newSeedId, randomSeedData.Id);

        return randomSeed;
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        IAccountStateDelta states = ctx.PreviousStates;

        InventoryState inventoryState =
            states.GetState(ctx.Signer) is Bencodex.Types.Dictionary stateEncoded
                ? new InventoryState(stateEncoded)
                : new InventoryState();

        SeedState seedState = generateRandomSeed(ctx.Random, inventoryState.NextSeedId);
        inventoryState = inventoryState.AddSeed(seedState);

        var encodedValue = inventoryState.ToBencodex();
        var statesWithUpdated = states.SetState(ctx.Signer, encodedValue);

        return statesWithUpdated;
    }
}
