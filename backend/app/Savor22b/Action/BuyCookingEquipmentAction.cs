namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;


[ActionType(nameof(BuyCookingEquipmentAction))]
public class BuyCookingEquipmentAction : SVRAction
{
    public Guid CookingEquipmentStateID;
    public int WantToBuyEquipmentID;

    public BuyCookingEquipmentAction()
    {
    }

    public BuyCookingEquipmentAction(Guid cookingEquipmentStateID, int wantToBuyEquipmentID)
    {
        CookingEquipmentStateID = cookingEquipmentStateID;
        WantToBuyEquipmentID = wantToBuyEquipmentID;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(CookingEquipmentStateID)] = CookingEquipmentStateID.Serialize(),
            [nameof(WantToBuyEquipmentID)] = WantToBuyEquipmentID.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        CookingEquipmentStateID = plainValue[nameof(CookingEquipmentStateID)].ToGuid();
        WantToBuyEquipmentID = plainValue[nameof(WantToBuyEquipmentID)].ToInteger();
    }

    private List<CookingEquipment> GetCookingEquipmentCSVData()
    {
        // CsvParser<Stat> csvParser = new CsvParser<Stat>();

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
        var cookingEquipment = csvData.Find(equipment => equipment.ID == WantToBuyEquipmentID);

        if (cookingEquipment is null)
        {
            throw new NotFoundTableDataException(
                $"Invalid {nameof(WantToBuyEquipmentID)}: {WantToBuyEquipmentID}");
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

        InventoryState inventoryState =
            states.GetState(ctx.Signer) is Bencodex.Types.Dictionary stateEncoded
                ? new InventoryState(stateEncoded)
                : new InventoryState();

        var cookingEquipmentList = GetCookingEquipmentCSVData();
        var wantToBuyEquipment = FindCookingEquipment(cookingEquipmentList);
        var cookingEquipmentState = new CookingEquipmentState(CookingEquipmentStateID, wantToBuyEquipment.ID);

        states = states.TransferAsset(
            ctx.Signer,
            Recipient,
            wantToBuyEquipment.Price,
            allowNegativeBalance: false
        );
        inventoryState = inventoryState.AddCookingEquipmentItem(cookingEquipmentState);

        return states.SetState(ctx.Signer, inventoryState.Serialize());
    }
}
