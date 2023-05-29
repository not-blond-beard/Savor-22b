namespace Savor22b.Action;

using System;
using Libplanet.Action;
using Libplanet.Store;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;


[ActionType("generate_ingredient")]
public class GenerateIngredientAction : BaseAction
{
    class ActionPlainValue : DataModel
    {
        public int SeedStateId { get; private set; }

        public ActionPlainValue(int seedStateId)
            : base()
        {
            SeedStateId = seedStateId;
        }

        public ActionPlainValue(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }
    }

    private ActionPlainValue _plainValue;

    public GenerateIngredientAction()
    {
    }

    public GenerateIngredientAction(int seedId)
    {
        _plainValue = new ActionPlainValue(seedId);
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

    private Ingredient? getMatchedIngredient(int seedId)
    {
        CsvParser<Ingredient> csvParser = new CsvParser<Ingredient>();

        var csvPath = Paths.GetCSVDataPath("ingredient.csv");
        var ingredients = csvParser.ParseCsv(csvPath);
        var matchedIngredient = ingredients.Find(ingredient => ingredient.SeedId == seedId);

        return matchedIngredient;
    }

    private Stat? getMatchedStat(int ingredientId, string grade)
    {
        CsvParser<Stat> csvParser = new CsvParser<Stat>();

        var csvPath = Paths.GetCSVDataPath("stat.csv");
        var stats = csvParser.ParseCsv(csvPath);

        var matchedStat = stats.Find(stat => stat.IngredientID == ingredientId && stat.Grade == grade);

        return matchedStat;
    }

    private RefrigeratorState generateIngredient(IActionContext ctx, int seedId, int refrigeratorCount)
    {
        var matchedIngredient = getMatchedIngredient(seedId);
        var gradeExtractor = new GradeExtractor(ctx.Random, 0.1);

        if (matchedIngredient == null)
        {
            throw new ArgumentException(
                $"Invalid {nameof(seedId)}: {seedId}");
        }

        var grade = gradeExtractor.ExtractGrade(matchedIngredient.MinGrade, matchedIngredient.MaxGrade);
        var gradeString = GradeExtractor.GetGrade(grade);
        var matchedStat = getMatchedStat(matchedIngredient.ID, gradeString);

        if (matchedStat == null)
        {
            throw new ArgumentException(
                $"Invalid {nameof(gradeString)}: {gradeString}");
        }

        var hp = ctx.Random.Next(matchedStat.MinHP, matchedStat.MaxHP + 1);
        var attack = ctx.Random.Next(matchedStat.MinAtk, matchedStat.MaxAtk + 1);
        var defense = ctx.Random.Next(matchedStat.MinDef, matchedStat.MaxDef + 1);
        var speed = ctx.Random.Next(matchedStat.MinSpd, matchedStat.MaxSpd + 1);

        var ingredient = new RefrigeratorState(
            refrigeratorCount,
            matchedIngredient.SeedId,
            null,
            gradeString,
            hp,
            attack,
            defense,
            speed
        );

        return ingredient;
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        IAccountStateDelta states = ctx.PreviousStates;

        InventoryState inventoryState =
            states.GetState(ctx.Signer) is Bencodex.Types.Dictionary stateEncoded
                ? new InventoryState(stateEncoded)
                : new InventoryState();

        var seed = inventoryState.SeedStateList.Find(seed => seed.Id == _plainValue.SeedStateId);

        if (seed == null)
        {
            throw new ArgumentException(
                $"Invalid {nameof(_plainValue.SeedStateId)}: {_plainValue.SeedStateId}");
        }

        var ingredient = generateIngredient(ctx, seed.SeedID, inventoryState.NextRefrigeratorId);

        inventoryState = inventoryState.AddRefrigeratorItem(ingredient);
        inventoryState = inventoryState.RemoveSeed(_plainValue.SeedStateId);

        return states.SetState(ctx.Signer, inventoryState.ToBencodex());
    }
}
