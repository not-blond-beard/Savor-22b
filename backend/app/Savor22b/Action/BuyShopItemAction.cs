namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Exceptions;
using Savor22b.Constants;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;

[ActionType(nameof(BuyShopItemAction))]
public class BuyShopItemAction : SVRAction
{
    public Guid ItemStateID;
    public int DesiredShopItemID;

    public BuyShopItemAction() { }

    public BuyShopItemAction(Guid itemStateID, int desiredShopItemID)
    {
        ItemStateID = itemStateID;
        DesiredShopItemID = desiredShopItemID;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(ItemStateID)] = ItemStateID.Serialize(),
            [nameof(DesiredShopItemID)] = DesiredShopItemID.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        ItemStateID = plainValue[nameof(ItemStateID)].ToGuid();
        DesiredShopItemID = plainValue[nameof(DesiredShopItemID)].ToInteger();
    }

    private Item FindRandomSeedItem()
    {
        var item = CsvDataHelper.GetItemByID(DesiredShopItemID);

        if (item is null)
        {
            throw new NotFoundTableDataException(
                $"Invalid {nameof(DesiredShopItemID)}: {DesiredShopItemID}"
            );
        }

        return item;
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        IAccountStateDelta states = ctx.PreviousStates;
        Address Recipient = Addresses.ShopVaultAddress;

        RootState rootState = states.GetState(ctx.Signer) is Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();
        var inventoryState = rootState.InventoryState;

        var desiredEquipment = FindRandomSeedItem();
        var itemState = new ItemState(ItemStateID, desiredEquipment.ID);

        states = states.TransferAsset(
            ctx.Signer,
            Recipient,
            desiredEquipment.PriceToFungibleAssetValue(),
            allowNegativeBalance: false
        );
        inventoryState = inventoryState.AddItem(itemState);
        rootState.SetInventoryState(inventoryState);

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
