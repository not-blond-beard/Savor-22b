namespace Savor22b.GraphTypes.Query;

using System.Collections.Immutable;
using System.Linq;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Savor22b.GraphTypes.Types;

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
        var kitchenEquipmentCSVData = CsvDataHelper.GetKitchenEquipmentCSVData().ToArray();
        var shopKitchenEquipments = kitchenEquipmentCSVData
            .Select(kitchenEquipment => new ShopKitchenEquipment(kitchenEquipment))
            .ToImmutableList();

        Shop shop = new Shop(shopKitchenEquipments);

        return shop;
    }
}
