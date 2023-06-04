namespace Savor22b.Tests.Action;

using System;
using Libplanet.Crypto;
using Libplanet.State;
using Xunit;
using Savor22b.Action;
using Savor22b.States;
using Libplanet;

public class GenerateSeedActionTests
{
    private PrivateKey _signer = new PrivateKey();

    public GenerateSeedActionTests()
    {
    }

    [Fact]
    public void GenerateSeedActionExecute_AddsSeedStateToList()
    {
        IAccountStateDelta state = new DummyState();
        var random = new DummyRandom(1);

        var action = new GenerateSeedAction();

        state = action.Execute(new DummyActionContext
        {
            PreviousStates = state,
            Signer = _signer.PublicKey.ToAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 1,
        });

        var inventoryStateEncoded = state.GetState(_signer.PublicKey.ToAddress());
        InventoryState inventoryState =
            inventoryStateEncoded is Bencodex.Types.Dictionary bdict
                ? new InventoryState(bdict)
                : throw new Exception();

        Assert.Equal(inventoryState.SeedStateList.Count, 1);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    public void GenerateSeedActionExecute_AddsSeedStateToExistsSeedsList(int existsSeedsListLength)
    {
        IAccountStateDelta state = new DummyState();
        InventoryState inventoryState = new InventoryState();

        for (int i = 0; i < existsSeedsListLength; i++)
        {
            var newSeed = new SeedState(Guid.NewGuid(), 1);
            inventoryState = inventoryState.AddSeed(newSeed);
        }

        state = state.SetState(_signer.PublicKey.ToAddress(), inventoryState.Serialize());

        var random = new DummyRandom(1);

        var action = new GenerateSeedAction(Guid.NewGuid());

        state = action.Execute(new DummyActionContext
        {
            PreviousStates = state,
            Signer = _signer.PublicKey.ToAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 1,
        });

        var afterInventoryStateEncoded = state.GetState(_signer.PublicKey.ToAddress());
        InventoryState afterInventoryState =
            afterInventoryStateEncoded is Bencodex.Types.Dictionary bdict
                ? new InventoryState(bdict)
                : throw new Exception();

        Assert.Equal(existsSeedsListLength + 1, afterInventoryState.SeedStateList.Count);
    }
}
