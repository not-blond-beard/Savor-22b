namespace Savor22b.GraphTypes.Types;

using Savor22b.Model;
using Savor22b.States;

public class KitchenEquipmentStateDetail
{
    public KitchenEquipmentStateDetail(KitchenEquipmentState kitchenEquipmentState)
    {
        StateID = kitchenEquipmentState.StateID;
        KitchenEquipmentID = kitchenEquipmentState.KitchenEquipmentID;
        KitchenEquipmentCategoryID = kitchenEquipmentState.KitchenEquipmentCategoryID;
        CookingStartedBlockIndex = kitchenEquipmentState.CookingStartedBlockIndex;
        CookingDurationBlock = kitchenEquipmentState.CookingDurationBlock;

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

    public long? CookingStartedBlockIndex { get; private set; }

    public long? CookingDurationBlock { get; private set; }
}
