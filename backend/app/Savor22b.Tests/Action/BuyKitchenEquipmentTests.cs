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

public class BuyKitchenEquipmentTests
{
    private PrivateKey _signer = new PrivateKey();

    public BuyKitchenEquipmentTests()
    {
    }

    [Fact]
    public void BuyKitchenEquipmentExecute_AddsKitchenEquipmentToKitchenStateList()
    {
        IAccountStateDelta state = new DummyState();
        state = state.MintAsset(
            _signer.PublicKey.ToAddress(),
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "2"
            ));

        var random = new DummyRandom(1);
        var desiredEquipmentID = 1;

        var action = new BuyKitchenEquipmentAction(Guid.NewGuid(), desiredEquipmentID);

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

        Assert.Equal(0, inventoryState.SeedStateList.Count);
        Assert.Equal(0, inventoryState.RefrigeratorStateList.Count);
        Assert.Equal(1, inventoryState.KitchenEquipmentStateList.Count);
        Assert.Equal(desiredEquipmentID, inventoryState.KitchenEquipmentStateList[0].KitchenEquipmentID);
        Assert.Equal(
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "0"
            ),
            state.GetBalance(_signer.PublicKey.ToAddress(), Currencies.KeyCurrency));
        Assert.Equal(
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "2"
            ),
            state.GetBalance(Addresses.ShopVaultAddress, Currencies.KeyCurrency));
    }
}
