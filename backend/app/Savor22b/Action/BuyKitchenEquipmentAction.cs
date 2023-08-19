namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Exceptions;
using Savor22b.Constants;
using Savor22b.Model;
using Savor22b.States;


[ActionType(nameof(BuyKitchenEquipmentAction))]
public class BuyKitchenEquipmentAction : SVRAction
{
    public Guid KitchenEquipmentStateID;
    public int DesiredEquipmentID;

    public BuyKitchenEquipmentAction()
    {
    }

    public BuyKitchenEquipmentAction(Guid kitchenEquipmentStateID, int desiredEquipmentID)
    {
        KitchenEquipmentStateID = kitchenEquipmentStateID;
        DesiredEquipmentID = desiredEquipmentID;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(KitchenEquipmentStateID)] = KitchenEquipmentStateID.Serialize(),
            [nameof(DesiredEquipmentID)] = DesiredEquipmentID.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        KitchenEquipmentStateID = plainValue[nameof(KitchenEquipmentStateID)].ToGuid();
        DesiredEquipmentID = plainValue[nameof(DesiredEquipmentID)].ToInteger();
    }

    private KitchenEquipment GetKitchenEquipment()
    {
        var kitchenEquipment = CsvDataHelper.GetKitchenEquipmentByID(DesiredEquipmentID);

        if (kitchenEquipment is null)
        {
            throw new NotFoundTableDataException(
                $"Invalid {nameof(DesiredEquipmentID)}: {DesiredEquipmentID}");
        }

        return kitchenEquipment;
    }

    private KitchenEquipmentCategory GetKitchenEquipmentCategory(int kitchenEquipmentCategoryID)
    {
        var kitchenEquipmentCategory = CsvDataHelper.GetKitchenEquipmentCategoryByID(kitchenEquipmentCategoryID);

        if (kitchenEquipmentCategory is null)
        {
            throw new NotFoundTableDataException(
                $"Invalid {nameof(DesiredEquipmentID)}: {DesiredEquipmentID}");
        }

        return kitchenEquipmentCategory;
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

        InventoryState inventoryState = rootState.InventoryState;

        var desiredEquipment = GetKitchenEquipment();
        var kitchenEquipmentCategory = GetKitchenEquipmentCategory(desiredEquipment.KitchenEquipmentCategoryID);
        var kitchenEquipmentState = new KitchenEquipmentState(KitchenEquipmentStateID, desiredEquipment.ID, kitchenEquipmentCategory.ID);

        states = states.TransferAsset(
            ctx.Signer,
            Recipient,
            desiredEquipment.PriceToFungibleAssetValue(),
            allowNegativeBalance: false
        );
        rootState.SetInventoryState(inventoryState.AddKitchenEquipmentItem(kitchenEquipmentState));

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
