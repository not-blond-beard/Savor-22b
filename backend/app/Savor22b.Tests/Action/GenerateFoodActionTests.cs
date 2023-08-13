namespace Savor22b.Tests.Action;

using System;
using System.Collections;
using System.Collections.Immutable;
using Humanizer;
using Libplanet;
using Libplanet.Crypto;
using Libplanet.State;
using Savor22b.Action;
using Savor22b.States;
using Savor22b.Tests.Fixtures;
using Xunit;


public class GenerateFoodActionTests : ActionTests, IClassFixture<CsvDataFixture>
{
    private CsvDataFixture _fixture;
    private PrivateKey _signer = new PrivateKey();

    public GenerateFoodActionTests(CsvDataFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GenerateFoodActionExecute_AddsFoodToRefrigeratorStateList(int recipeID)
    {
        IAccountStateDelta beforeState = new DummyState();

        var recipe = _fixture.Recipes.Find(recipe => recipe.ID == recipeID);

        if (recipe is null)
        {
            throw new Exception();
        }

        InventoryState beforeInventoryState = new InventoryState();
        foreach (var ingredientID in recipe.IngredientIDList)
        {
            beforeInventoryState = beforeInventoryState.AddRefrigeratorItem(
                RefrigeratorState.CreateIngredient(
                    Guid.NewGuid(),
                    ingredientID,
                    "D",
                    1,
                    1,
                    1,
                    1
                ));
        }
        foreach (var foodID in recipe.FoodIDList)
        {
            beforeInventoryState = beforeInventoryState.AddRefrigeratorItem(
                RefrigeratorState.CreateFood(
                    Guid.NewGuid(),
                    foodID,
                    "D",
                    1,
                    1,
                    1,
                    1
                ));
        }
        var beforeRootState = new RootState(beforeInventoryState);

        beforeState = beforeState.SetState(_signer.PublicKey.ToAddress(), beforeRootState.Serialize());

        var random = new DummyRandom(1);

        var newFoodGuid = Guid.NewGuid();
        var action = new GenerateFoodAction(
            recipe.ID,
            newFoodGuid,
            (from stateList in beforeRootState.InventoryState.RefrigeratorStateList
             select stateList.StateID).ToList());

        var afterState = action.Execute(new DummyActionContext
        {
            PreviousStates = beforeState,
            Signer = _signer.PublicKey.ToAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 1,
        });

        var rootStateEncoded = afterState.GetState(_signer.PublicKey.ToAddress());
        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();
        InventoryState afterInventoryState = rootState.InventoryState;

        Assert.Equal(afterInventoryState.RefrigeratorStateList.Count, 1);
        Assert.Equal(afterInventoryState.RefrigeratorStateList[0].FoodID, recipe.ResultFoodID);
    }
}
