namespace Savor22b.GraphTypes.Query;

using System.Collections.Immutable;
using System.Linq;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Savor22b.GraphTypes.Types;
using Savor22b.Model;

public class ShopQuery : FieldType
{
    public ShopQuery()
        : base()
    {
        Name = "shop";
        Type = typeof(NonNullGraphType<ShopType>);
        Description = "This contains information about the items sold in the shop.";
        Resolver = new FuncFieldResolver<Shop>(context =>
        {
            try
            {
                Shop shop = GetShop();
                return shop;
            }
            catch (Exception e)
            {
                throw new ExecutionError(e.Message);
            }
        });
    }

    private static Shop GetShop()
    {
        var kitchenEquipmentCSVData = CsvDataHelper.GetKitchenEquipmentCSVData();
        var shopKitchenEquipments = kitchenEquipmentCSVData
            .Select(kitchenEquipment => new ShopKitchenEquipment(kitchenEquipment))
            .ToImmutableList();

        var itemCSVData = CsvDataHelper.GetItemCSVData();
        var shopItems = itemCSVData
            .Select(item =>
            {
                var shopItem = new Item
                {
                    ID = item.ID,
                    Name = item.Name,
                    Price = item.Price,
                };

                return item;
            })
            .ToImmutableList();

        Shop shop = new Shop(shopKitchenEquipments, shopItems);

        return shop;
    }
}
