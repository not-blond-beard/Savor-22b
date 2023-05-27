namespace Savor22b.Action;

using System;
using Libplanet.Action;
using Libplanet.Store;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;


[ActionType("generate_seed")]
public class GenerateSeedAction : BaseAction
{

    class ActionPlainValue : DataModel
    {

        public ActionPlainValue()
            : base()
        {
        }


        public ActionPlainValue(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }
    }

    private ActionPlainValue _plainValue;

    public GenerateSeedAction()
    {
        _plainValue = new ActionPlainValue();
    }

    public override Bencodex.Types.IValue PlainValue => _plainValue.Encode();

    public override void LoadPlainValue(Bencodex.Types.IValue plainValue)
    {
        if (plainValue is Bencodex.Types.Dictionary bdict)
        {
            _plainValue = new ActionPlainValue(bdict);
        }
        else
        {
            throw new ArgumentException(
                $"Invalid {nameof(plainValue)} type: {plainValue.GetType()}");
        }
    }

    private SeedState generateRandomSeed(IRandom random)
    {
        CsvParser<Seed> csvParser = new CsvParser<Seed>();

        var csvPath = Paths.GetCSVDataPath("seed.csv");

        var seeds = csvParser.ParseCsv(csvPath);
        int randomIndex = random.Next(0, seeds.Count);

        var randomSeedData = seeds[randomIndex];
        var randomSeed = new SeedState(randomSeedData.Id, randomSeedData.Name);

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

        var encodedValue = inventoryState.ToBencodex();
        var statesWithUpdated = states.SetState(ctx.Signer, encodedValue);

        return statesWithUpdated;
    }
}
