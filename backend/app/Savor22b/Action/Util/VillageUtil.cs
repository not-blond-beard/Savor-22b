using Savor22b.Model;

namespace Savor22b.Action.Util;

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
}
