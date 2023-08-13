namespace Savor22b.Model;
using Libplanet.Assets;

public class Item
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Price { get; set; }

    public FungibleAssetValue PriceToFungibleAssetValue()
    {
        return FungibleAssetValue.Parse(Currencies.KeyCurrency, Price);
    }
}
