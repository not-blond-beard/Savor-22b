using System.Collections.Immutable;

namespace Savor22b.GraphTypes.Types;

public class Shop
{
    public ImmutableList<ShopKitchenEquipment> KitchenEquipments { get; private set; }

    public Shop(ImmutableList<ShopKitchenEquipment> kitchenEquipments)
    {
        KitchenEquipments = kitchenEquipments;
    }
}
