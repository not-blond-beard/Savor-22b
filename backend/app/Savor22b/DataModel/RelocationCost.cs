namespace Savor22b.DataModel;

using Libplanet.Assets;

public class RelocationCost
{
    public int DurationBlocks { get; set; }
    public FungibleAssetValue Price { get; set; }

    public RelocationCost(int durationBlocks, FungibleAssetValue price)
    {
        DurationBlocks = durationBlocks;
        Price = price;
    }
}
