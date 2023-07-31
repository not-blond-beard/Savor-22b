namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;
using Libplanet.State;
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

    private List<CookingEquipment> GetCookingEquipmentCSVData()
    {
        // CsvParser<CookingEquipment> csvParser = new CsvParser<CookingEquipment>();

        // var csvPath = Paths.GetCSVDataPath("cooking-equipment.csv");
        // var cookingEquipment = csvParser.ParseCsv(csvPath);
        var cookingEquipmentList = new List<CookingEquipment>();
        var cookingEquipment = new CookingEquipment();
        cookingEquipment.ID = 1;
        cookingEquipment.Name = "TempData";
        cookingEquipment.BlockTimeReductionPercent = 0.05;
        cookingEquipment.Price = FungibleAssetValue.Parse(Currencies.KeyCurrency, "10");

        cookingEquipmentList.Add(cookingEquipment);

        return cookingEquipmentList;
    }

    private CookingEquipment FindCookingEquipment(List<CookingEquipment> csvData)
    {
        var cookingEquipment = csvData.Find(equipment => equipment.ID == DesiredEquipmentID);

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

        var cookingEquipmentList = GetCookingEquipmentCSVData();
        var desiredEquipment = FindCookingEquipment(cookingEquipmentList);
        var cookingEquipmentState = new CookingEquipmentState(CookingEquipmentStateID, desiredEquipment.ID);

        states = states.TransferAsset(
            ctx.Signer,
            Recipient,
            desiredEquipment.Price,
            allowNegativeBalance: false
        );
        rootState.SetInventoryState(inventoryState.AddCookingEquipmentItem(cookingEquipmentState));

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
