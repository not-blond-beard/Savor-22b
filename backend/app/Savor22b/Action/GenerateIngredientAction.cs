namespace Savor22b.Action;

using System.Collections.Immutable;
using Bencodex.Types;
using System;
using Libplanet.Action;
using Libplanet.State;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;
using Libplanet.Headless.Extensions;


[ActionType(nameof(GenerateIngredientAction))]
public class GenerateIngredientAction : SVRAction
{
    public Guid RefrigeratorStateID;
    public Guid SeedStateID;

    public GenerateIngredientAction()
    {
    }

    public GenerateIngredientAction(Guid seedStateID, Guid refrigeratorStateID)
    {
        SeedStateID = seedStateID;
        RefrigeratorStateID = refrigeratorStateID;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>(){
            [nameof(SeedStateID)] = SeedStateID.Serialize(),
            [nameof(RefrigeratorStateID)] = RefrigeratorStateID.Serialize()
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        SeedStateID = plainValue[nameof(SeedStateID)].ToGuid();
        RefrigeratorStateID = plainValue[nameof(RefrigeratorStateID)].ToGuid();
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

    private RefrigeratorState generateIngredient(IActionContext ctx, int seedId)
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
            RefrigeratorStateID,
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

        var seed = inventoryState.SeedStateList.Find(seed => seed.StateID == SeedStateID);

        if (seed == null)
        {
            throw new ArgumentException(
                $"Invalid {nameof(SeedStateID)}: {SeedStateID}");
        }

        var ingredient = generateIngredient(ctx, seed.SeedID);

        inventoryState = inventoryState.AddRefrigeratorItem(ingredient);
        inventoryState = inventoryState.RemoveSeed(SeedStateID);

        return states.SetState(ctx.Signer, inventoryState.Serialize());
    }
}
