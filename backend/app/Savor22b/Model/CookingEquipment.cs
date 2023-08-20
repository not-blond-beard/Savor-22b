namespace Savor22b.Model;

using Libplanet.Assets;

public class KitchenEquipment
{
    public int ID { get; set; }
    public int KitchenEquipmentCategoryID { get; set; }
    public string Name { get; set; }
    public int BlockTimeReductionPercent { get; set; }
    public string Price { get; set; }

    public FungibleAssetValue PriceToFungibleAssetValue()
    {
        return FungibleAssetValue.Parse(Currencies.KeyCurrency, Price);
    }
}
