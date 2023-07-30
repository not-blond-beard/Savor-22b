namespace Savor22b.Tests.Action;

using System;
using Libplanet;
using Libplanet.Assets;
using Libplanet.Crypto;
using Libplanet.State;
using Savor22b.Action;
using Savor22b.Constants;
using Savor22b.States;
using Xunit;

public class BuyRandomSeedItemActionTests : ActionTests
{

    public BuyRandomSeedItemActionTests()
    {
    }

    [Fact]
    public void BuyRandomSeedItemActionExecute_AddsItemToItemStateList()
    {
        IAccountStateDelta state = new DummyState();
        state = state.MintAsset(
            SignerAddress(),
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "10"
            ));

        var random = new DummyRandom(1);
        var desiredRandomSeedItemID = 1;

        var action = new BuyRandomSeedItemAction(Guid.NewGuid(), desiredRandomSeedItemID);

        state = action.Execute(new DummyActionContext
        {
            PreviousStates = state,
            Signer = SignerAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 1,
        });

        var inventoryStateEncoded = state.GetState(SignerAddress());
        InventoryState inventoryState =
            inventoryStateEncoded is Bencodex.Types.Dictionary bdict
                ? new InventoryState(bdict)
                : throw new Exception();

        Assert.Equal(0, inventoryState.SeedStateList.Count);
        Assert.Equal(0, inventoryState.RefrigeratorStateList.Count);
        Assert.Equal(0, inventoryState.CookingEquipmentStateList.Count);
        Assert.Equal(1, inventoryState.ItemStateList.Count);
        Assert.Equal(desiredRandomSeedItemID, inventoryState.ItemStateList[0].ItemID);
        Assert.Equal(
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "0"
            ),
            state.GetBalance(SignerAddress(), Currencies.KeyCurrency));
        Assert.Equal(
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "10"
            ),
            state.GetBalance(Addresses.ShopVaultAddress, Currencies.KeyCurrency));
    }
}
