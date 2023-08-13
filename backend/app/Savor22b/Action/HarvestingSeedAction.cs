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

[ActionType(nameof(HarvestingSeedAction))]
public class HarvestingSeedAction : SVRAction
{
    public int HarvestedFieldIndex;
    public Guid RefrigeratorStateID;

    public HarvestingSeedAction()
    {

    }

    public HarvestingSeedAction(int harvestedFieldIndex, Guid refrigeratorStateID)
    {
        HarvestedFieldIndex = harvestedFieldIndex;
        RefrigeratorStateID = refrigeratorStateID;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(HarvestedFieldIndex)] = HarvestedFieldIndex.Serialize(),
            [nameof(RefrigeratorStateID)] = RefrigeratorStateID.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        HarvestedFieldIndex = plainValue[nameof(HarvestedFieldIndex)].ToInteger();
        RefrigeratorStateID = plainValue[nameof(RefrigeratorStateID)].ToGuid();
    }

    private VillageState getVillageState(RootState rootState)
    {
        if (rootState.VillageState is null)
        {
            throw new InvalidVillageStateException("VillageState is null");
        }

        return rootState.VillageState;
    }

    private HouseFieldState getHouseFieldState(VillageState villageState, int index)
    {
        HouseFieldState houseFieldState = villageState.HouseFieldStates[index] ?? throw new InvalidFieldIndexException("FieldIndex is invalid");

        return houseFieldState;
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

        var ingredient = RefrigeratorState.CreateIngredient(
            RefrigeratorStateID,
            matchedIngredient.SeedId,
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
        IAccountStateDelta states = ctx.PreviousStates;
        RootState rootState = states.GetState(ctx.Signer) is Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();

        VillageState villageState = getVillageState(rootState);
        InventoryState inventoryState = rootState.InventoryState;
        HouseFieldState houseFieldState = getHouseFieldState(villageState, HarvestedFieldIndex);

        if (!houseFieldState.IsHarvestable(ctx.BlockIndex))
        {
            throw new HarvestableFieldException("Field is not harvestable");
        }

        villageState.RemoveHouseFieldState(HarvestedFieldIndex);

        RefrigeratorState ingredient = generateIngredient(
            ctx,
            houseFieldState.SeedID
        );
        inventoryState = inventoryState.AddRefrigeratorItem(ingredient);

        rootState.SetInventoryState(inventoryState);

        states = states.SetState(ctx.Signer, rootState.Serialize());

        return states;
    }

}
