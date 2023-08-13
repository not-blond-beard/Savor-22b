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


[ActionType(nameof(BuyCookingEquipmentAction))]
public class BuyCookingEquipmentAction : SVRAction
{
    public Guid CookingEquipmentStateID;
    public int DesiredEquipmentID;

    public BuyCookingEquipmentAction()
    {
    }

    public BuyCookingEquipmentAction(Guid cookingEquipmentStateID, int desiredEquipmentID)
    {
        CookingEquipmentStateID = cookingEquipmentStateID;
        DesiredEquipmentID = desiredEquipmentID;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(CookingEquipmentStateID)] = CookingEquipmentStateID.Serialize(),
            [nameof(DesiredEquipmentID)] = DesiredEquipmentID.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        CookingEquipmentStateID = plainValue[nameof(CookingEquipmentStateID)].ToGuid();
        DesiredEquipmentID = plainValue[nameof(DesiredEquipmentID)].ToInteger();
    }

    private CookingEquipment FindCookingEquipment()
    {
        var cookingEquipment = CsvDataHelper.GetCookingEquipmentByID(DesiredEquipmentID);

        if (cookingEquipment is null)
        {
            throw new NotFoundTableDataException(
                $"Invalid {nameof(DesiredEquipmentID)}: {DesiredEquipmentID}");
        }

        return cookingEquipment;
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        IAccountStateDelta states = ctx.PreviousStates;
        Address Recipient = Addresses.ShopVaultAddress;

        RootState rootState = states.GetState(ctx.Signer) is Bencodex.Types.Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();

        InventoryState inventoryState = rootState.InventoryState;

        var desiredEquipment = FindCookingEquipment();
        var cookingEquipmentState = new CookingEquipmentState(CookingEquipmentStateID, desiredEquipment.ID);

        states = states.TransferAsset(
            ctx.Signer,
            Recipient,
            desiredEquipment.PriceToFungibleAssetValue(),
            allowNegativeBalance: false
        );
        rootState.SetInventoryState(inventoryState.AddCookingEquipmentItem(cookingEquipmentState));

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
