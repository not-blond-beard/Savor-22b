namespace Savor22b.Model;
using Libplanet.Assets;

public class CookingEquipment
{
    public int ID { get; set; }
    public FungibleAssetValue Price { get; set; }
    public string Name { get; set; }
    public double BlockTimeReductionPercent { get; set; }
}
