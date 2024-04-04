namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.States;
using Libplanet.Assets;

[ActionType(nameof(ShowMeTheMoneyAction))]
public class ShowMeTheMoneyAction : SVRAction
{
    public Address Address;

    public ShowMeTheMoneyAction() { }

    public ShowMeTheMoneyAction(Address address)
    {
        Address = address;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(Address)] = Address.ToBencodex(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        Address = plainValue[nameof(Address)].ToAddress();
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        IAccountStateDelta states = ctx.PreviousStates;

        RootState rootState = states.GetState(Address) is Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();

        var inventoryState = rootState.InventoryState;

        for (int i = 0; i < 10; i++)
        {
            foreach (var seed in CsvDataHelper.GetSeedCSVData())
            {
                inventoryState = inventoryState.AddSeed(new SeedState(ctx.Random.GenerateRandomGuid(), seed.Id));
                rootState.SetInventoryState(inventoryState);
            }

            foreach (var ingredient in CsvDataHelper.GetIngredientCSVData())
            {
                inventoryState = inventoryState.AddRefrigeratorItem(
                    RefrigeratorState.CreateIngredient(
                        ctx.Random.GenerateRandomGuid(),
                        ingredient.ID,
                        "A",
                        10,
                        10,
                        10,
                        10
                    ));
                rootState.SetInventoryState(inventoryState);
            }

            foreach (var food in CsvDataHelper.GetFoodCSVData())
            {
                inventoryState = inventoryState.AddRefrigeratorItem(
                    RefrigeratorState.CreateFood(
                        ctx.Random.GenerateRandomGuid(),
                        food.ID,
                        "A",
                        10,
                        10,
                        10,
                        10,
                        1,
                        new List<Guid>().ToImmutableList()
                    ));
                rootState.SetInventoryState(inventoryState);
            }

            foreach (var kitchenEquipment in CsvDataHelper.GetKitchenEquipmentCSVData())
            {
                inventoryState = inventoryState.AddKitchenEquipmentItem(
                    new KitchenEquipmentState(ctx.Random.GenerateRandomGuid(), kitchenEquipment.ID, kitchenEquipment.KitchenEquipmentCategoryID));
                rootState.SetInventoryState(rootState.InventoryState);
            }
        }

        for (int i = 0; i < 100; i++)
        {
            foreach (var item in CsvDataHelper.GetItemCSVData())
            {
                inventoryState = inventoryState.AddItem(new ItemState(ctx.Random.GenerateRandomGuid(), item.ID));
                rootState.SetInventoryState(inventoryState);
            }
        }

        states = states.MintAsset(Address, FungibleAssetValue.Parse(
            Currencies.KeyCurrency,
            "10000"
        ));
        return states.SetState(Address, rootState.Serialize());
    }
}
