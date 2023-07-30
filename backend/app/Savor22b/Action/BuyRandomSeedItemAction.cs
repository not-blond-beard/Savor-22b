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


[ActionType(nameof(BuyRandomSeedItemAction))]
public class BuyRandomSeedItemAction : SVRAction
{
    public Guid RandomSeedItemStateID;
    public int DesiredRandomSeedItemID;

    public BuyRandomSeedItemAction()
    {
    }

    public BuyRandomSeedItemAction(Guid randomSeedItemStateID, int desiredRandomSeedItemID)
    {
        RandomSeedItemStateID = randomSeedItemStateID;
        DesiredRandomSeedItemID = desiredRandomSeedItemID;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(RandomSeedItemStateID)] = RandomSeedItemStateID.Serialize(),
            [nameof(DesiredRandomSeedItemID)] = DesiredRandomSeedItemID.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        RandomSeedItemStateID = plainValue[nameof(RandomSeedItemStateID)].ToGuid();
        DesiredRandomSeedItemID = plainValue[nameof(DesiredRandomSeedItemID)].ToInteger();
    }

    private List<Item> GetRandomSeedItemCSVData()
    {
        // CsvParser<Stat> csvParser = new CsvParser<Stat>();

        // var csvPath = Paths.GetCSVDataPath("cooking-equipment.csv");
        // var randomSeedItem = csvParser.ParseCsv(csvPath);
        var randomSeedItemList = new List<Item>();
        var randomSeedItem = new Item();
        randomSeedItem.ID = 1;
        randomSeedItem.Name = "TempData";
        randomSeedItem.Price = FungibleAssetValue.Parse(Currencies.KeyCurrency, "10");

        randomSeedItemList.Add(randomSeedItem);

        return randomSeedItemList;
    }

    private Item FindRandomSeedItem(List<Item> csvData)
    {
        var randomSeedItem = csvData.Find(equipment => equipment.ID == DesiredRandomSeedItemID);

        if (randomSeedItem is null)
        {
            throw new NotFoundTableDataException(
                $"Invalid {nameof(DesiredRandomSeedItemID)}: {DesiredRandomSeedItemID}");
        }

        return randomSeedItem;
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

        var randomSeedItemList = GetRandomSeedItemCSVData();
        var desiredEquipment = FindRandomSeedItem(randomSeedItemList);
        var randomSeedItemState = new ItemState(RandomSeedItemStateID, desiredEquipment.ID);

        states = states.TransferAsset(
            ctx.Signer,
            Recipient,
            desiredEquipment.Price,
            allowNegativeBalance: false
        );
        inventoryState = inventoryState.AddItem(randomSeedItemState);

        return states.SetState(ctx.Signer, inventoryState.Serialize());
    }
}
