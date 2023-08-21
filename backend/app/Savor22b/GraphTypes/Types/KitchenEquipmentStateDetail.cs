namespace Savor22b.GraphTypes.Types;

using Savor22b.Model;
using Savor22b.States;

public class KitchenEquipmentStateDetail
{
    public KitchenEquipmentStateDetail(
        KitchenEquipmentState kitchenEquipmentState,
        InventoryState inventoryState,
        long currentBlockIndex
    )
    {
        StateID = kitchenEquipmentState.StateID;
        KitchenEquipmentID = kitchenEquipmentState.KitchenEquipmentID;
        KitchenEquipmentCategoryID = kitchenEquipmentState.KitchenEquipmentCategoryID;

        var isInUse = kitchenEquipmentState.IsInUse(currentBlockIndex);
        IsCooking = isInUse;
        if (isInUse)
        {
            CookingEndBlockIndex = kitchenEquipmentState.CookingEndBlockIndex();
            var cookingFood = inventoryState.GetRefrigeratorItem(
                kitchenEquipmentState.CookingFoodStateID!.Value
            );

            if (cookingFood == null)
            {
                throw new Exception("Not found cooking food state id");
            }

            CookingFood = cookingFood;
        }
        else
        {
            CookingEndBlockIndex = null;
            CookingFood = null;
        }

        KitchenEquipment? kitchenEquipment = CsvDataHelper.GetKitchenEquipmentByID(
            KitchenEquipmentID
        );

        if (kitchenEquipment == null)
        {
            throw new Exception($"KitchenEquipment not found. ID: {KitchenEquipmentID}");
        }

        EquipmentName = kitchenEquipment.Name;
        BlockTimeReductionPercent = kitchenEquipment.BlockTimeReductionPercent;

        KitchenEquipmentCategory? kitchenEquipmentCategory =
            CsvDataHelper.GetKitchenEquipmentCategoryByID(KitchenEquipmentCategoryID);

        if (kitchenEquipmentCategory == null)
        {
            throw new Exception(
                $"KitchenEquipmentCategory not found. ID: {KitchenEquipmentCategoryID}"
            );
        }

        EquipmentCategoryName = kitchenEquipmentCategory.Name;
        EquipmentCategoryType = kitchenEquipmentCategory.Category;
    }

    public Guid StateID { get; private set; }

    public string EquipmentName { get; private set; }

    public double BlockTimeReductionPercent { get; private set; }

    public string EquipmentCategoryName { get; private set; }

    public string EquipmentCategoryType { get; private set; }

    public int KitchenEquipmentID { get; private set; }

    public int KitchenEquipmentCategoryID { get; private set; }

    public bool IsCooking { get; private set; }

    public long? CookingEndBlockIndex { get; private set; }

    public RefrigeratorState? CookingFood { get; private set; }
}
