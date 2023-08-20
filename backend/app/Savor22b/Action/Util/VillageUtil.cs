namespace Savor22b.Action.Util;

using Libplanet.Assets;
using Savor22b.DataModel;
using Savor22b.Model;

public static class VillageUtil
{
    public static int CalculateReplaceUserHouseBlock(
        int originVillageTargetX,
        int originVillageTargetY,
        int villageTargetX,
        int villageTargetY
    )
    {
        double distance = MathUtil.CalculateEuclideanDistance(
            originVillageTargetX,
            originVillageTargetY,
            villageTargetX,
            villageTargetY
        );

        int block = (int)distance * Village.DistanceBlockUnit;

        return block;
    }

    public static RelocationCost CalculateRelocationCost(
        int originVillageTargetX,
        int originVillageTargetY,
        int villageTargetX,
        int villageTargetY
    )
    {
        int block = CalculateReplaceUserHouseBlock(
            originVillageTargetX,
            originVillageTargetY,
            villageTargetX,
            villageTargetY
        );

        FungibleAssetValue price = CalculatePrice.CalculateReplaceUserHousePrice(
            originVillageTargetX,
            originVillageTargetY,
            villageTargetX,
            villageTargetY
        );

        return new RelocationCost(block, price);
    }
}
