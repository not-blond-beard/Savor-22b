using System.Collections.Immutable;
using Savor22b.Model;

namespace Savor22b.GraphTypes.Types;

public class Shop
{
    public ImmutableList<ShopKitchenEquipment> KitchenEquipments { get; private set; }

    public ImmutableList<Item> Items { get; private set; }

    public Shop(
        ImmutableList<ShopKitchenEquipment> kitchenEquipments,
        ImmutableList<Item> itemStateDetails
    )
    {
        KitchenEquipments = kitchenEquipments;
        Items = itemStateDetails;
    }
}
