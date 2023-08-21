using System.Collections.Immutable;
using Savor22b.Model;

namespace Savor22b.GraphTypes.Types;

public class ShopKitchenEquipment
{
    public ShopKitchenEquipment(KitchenEquipment kitchenEquipment)
    {
        ID = kitchenEquipment.ID;
        CategoryID = kitchenEquipment.KitchenEquipmentCategoryID;
        Name = kitchenEquipment.Name;
        BlockTimeReductionPercent = kitchenEquipment.BlockTimeReductionPercent;
        Price = kitchenEquipment.Price;

        KitchenEquipmentCategory? kitchenEquipmentCategory =
            CsvDataHelper.GetKitchenEquipmentCategoryByID(CategoryID);

        if (kitchenEquipmentCategory == null)
        {
            throw new Exception($"KitchenEquipmentCategory not found. ID: {CategoryID}");
        }

        CategoryLabel = kitchenEquipmentCategory.Name;
        CategoryType = kitchenEquipmentCategory.Category;
    }

    public int ID { get; private set; }

    public int CategoryID { get; private set; }

    public string CategoryLabel { get; private set; }

    public string CategoryType { get; private set; }

    public string Name { get; private set; }

    public double BlockTimeReductionPercent { get; private set; }

    public string Price { get; private set; }
}
