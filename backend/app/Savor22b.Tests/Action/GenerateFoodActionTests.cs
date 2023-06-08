namespace Savor22b.Tests.Action;

using System;
using System.Collections;
using System.Collections.Immutable;
using Libplanet;
using Libplanet.Crypto;
using Libplanet.State;
using Savor22b.Action;
using Savor22b.States;
using Savor22b.Tests.Fixtures;
using Xunit;


public class IngredientsSamplesData : IEnumerable<object[]>
{
    private CsvDataFixture _fixture;

    public IngredientsSamplesData()
    {
        _fixture = new CsvDataFixture();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        var recipeIDs = new[] { 1, 2, 3 };
        foreach (var recipeID in recipeIDs)
        {
            var preset = CreateRefrigeratorStatePreSet(recipeID);
            yield return new object[] { recipeID, preset.Item1, preset.Item2 };
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private (List<int>, List<int>) CreateRefrigeratorStatePreSet(int recipeID)
    {
        var data = _fixture.RecipeReferences;

        var recipe1 = data.FindAll(recipe => recipe.ID == recipeID);
        var expectUsedIngredientIDs = from r1 in recipe1
                                      where r1.IngredientID.HasValue
                                      select r1.IngredientID.Value;
        var expectUsedFoodIDs = from r1 in recipe1
                                where r1.ReferencedRecipeID.HasValue
                                select r1.ReferencedRecipeID.Value;

        return (expectUsedIngredientIDs.ToList(), expectUsedFoodIDs.ToList());
    }

}

public class GenerateFoodActionTests : IClassFixture<CsvDataFixture>
{
    CsvDataFixture _fixture;
    private PrivateKey _signer = new PrivateKey();

    public GenerateFoodActionTests(CsvDataFixture fixture)
    {
        _fixture = fixture;
    }

    public (List<int>, List<int>) CreateRefrigeratorStatePreSet(int recipeID)
    {
        var data = _fixture.RecipeReferences;

        var recipe1 = data.FindAll(recipe => recipe.ID == recipeID);
        var expectUsedIngredientIDs = from r1 in recipe1
                                      where r1.IngredientID is not null
                                      select r1.IngredientID;
        var expectUsedFoodIDs = from r1 in recipe1
                                where r1.ReferencedRecipeID is not null
                                select r1.ReferencedRecipeID;

        return ((List<int>)expectUsedIngredientIDs, (List<int>)expectUsedFoodIDs);
    }

    public IEnumerable<object[]> IngredientsSamples()
    {
        var recipeID = 1;
        var preset = CreateRefrigeratorStatePreSet(recipeID);
        yield return new object[] { recipeID, preset.Item1, preset.Item2 };
        recipeID = 2;
        preset = CreateRefrigeratorStatePreSet(recipeID);
        yield return new object[] { recipeID, preset.Item1, preset.Item2 };
        recipeID = 3;
        preset = CreateRefrigeratorStatePreSet(recipeID);
        yield return new object[] { recipeID, preset.Item1, preset.Item2 };
    }

    public InventoryState CreateInventoryStateFromExpectValues(List<int> expectUsedIngredientIDs, List<int> expectUsedFoodIDs)
    {
        InventoryState inventoryState = new InventoryState();

        foreach (var ingredientID in expectUsedIngredientIDs)
        {
            inventoryState = inventoryState.AddRefrigeratorItem(RefrigeratorState.CreateIngredient(Guid.NewGuid(), ingredientID, "D", 1, 1, 1, 1));
        }
        foreach (var foodID in expectUsedFoodIDs)
        {
            inventoryState = inventoryState.AddRefrigeratorItem(RefrigeratorState.CreateFood(Guid.NewGuid(), foodID, "D", 1, 1, 1, 1));
        }

        return inventoryState;
    }

    [Theory]
    [ClassData(typeof(IngredientsSamplesData))]
    public void GenerateFoodActionExecute_AddsFoodToRefrigeratorStateList(int expectRecipeID, List<int> expectUsedIngredients, List<int> expectUsedFoods)
    {
        IAccountStateDelta beforeState = new DummyState();
        var beforeInventoryState = CreateInventoryStateFromExpectValues(expectUsedIngredients, expectUsedFoods);
        beforeState = beforeState.SetState(_signer.PublicKey.ToAddress(), beforeInventoryState.Serialize());

        var random = new DummyRandom(1);

        var newFoodGuid = Guid.NewGuid();
        var action = new GenerateFoodAction(
            expectRecipeID,
            newFoodGuid,
            (from stateList in beforeInventoryState.RefrigeratorStateList
             select stateList.StateID).ToList());

        var afterState = action.Execute(new DummyActionContext
        {
            PreviousStates = beforeState,
            Signer = _signer.PublicKey.ToAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 1,
        });

        var inventoryStateEncoded = afterState.GetState(_signer.PublicKey.ToAddress());
        InventoryState afterInventoryState =
            inventoryStateEncoded is Bencodex.Types.Dictionary bdict
                ? new InventoryState(bdict)
                : throw new Exception();

        Assert.Equal(afterInventoryState.RefrigeratorStateList.Count, 1);
        Assert.Equal(afterInventoryState.RefrigeratorStateList[0].RecipeID, expectRecipeID);
    }
}
