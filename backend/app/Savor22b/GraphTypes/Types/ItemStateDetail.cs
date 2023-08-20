namespace Savor22b.GraphTypes.Types;

using Savor22b.Model;
using Savor22b.States;

public class ItemStateDetail
{
    public Guid StateID;

    public int ItemID;

    public string ItemName;

    public ItemStateDetail(ItemState itemState)
    {
        StateID = itemState.StateID;
        ItemID = itemState.ItemID;

        Item? item = CsvDataHelper.GetItemByID(ItemID);

        if (item == null)
        {
            throw new Exception($"Item not found. ID: {ItemID}");
        }

        ItemName = item.Name;
    }
}
