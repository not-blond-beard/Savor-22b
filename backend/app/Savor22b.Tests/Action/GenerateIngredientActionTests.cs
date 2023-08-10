namespace Savor22b.Tests.Action;

using System;
using Libplanet.Crypto;
using Libplanet.State;
using Xunit;
using Savor22b.Action;
using Savor22b.States;
using Libplanet;

public class GenerateIngredientActionTests
{
    private PrivateKey _signer = new PrivateKey();

    public GenerateIngredientActionTests()
    {
    }

    [Fact]
    public void GenerateIngredientActionExecute_AddsIngredientToRefrigeratorStateList()
    {
        IAccountStateDelta state = new DummyState();
        (state, var addsSeedStateID) = AddSeedState(state);

        var random = new DummyRandom(1);

        var action = new GenerateIngredientAction(addsSeedStateID, Guid.NewGuid());

        state = action.Execute(new DummyActionContext
        {
            PreviousStates = state,
            Signer = _signer.PublicKey.ToAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 1,
        });

        var rootStateEncoded = state.GetState(_signer.PublicKey.ToAddress());
        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();
        InventoryState inventoryState = rootState.InventoryState;

        Assert.Equal(inventoryState.SeedStateList.Count, 0);
        Assert.Equal(inventoryState.RefrigeratorStateList.Count, 1);
    }

    private (IAccountStateDelta, Guid) AddSeedState(IAccountStateDelta state)
    {
        RootState rootState = new RootState();
        InventoryState inventoryState = new InventoryState();
        var newSeed = new SeedState(Guid.NewGuid(), 1);
        inventoryState = inventoryState.AddSeed(newSeed);

        rootState.SetInventoryState(inventoryState);

        return (state.SetState(_signer.PublicKey.ToAddress(), rootState.Serialize()), newSeed.StateID);
    }
}
