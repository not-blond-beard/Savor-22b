namespace Savor22b.Action.Util;

using System.Globalization;
using Libplanet.Assets;
using Savor22b.Model;
using Savor22b.Util;

public static class CalculatePrice
{
    public static FungibleAssetValue CalculateReplaceUserHousePrice(
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
        int price = (int)distance * Village.DistancePriceUnit;

        return FungibleAssetValue.Parse(
            Currencies.KeyCurrency,
            price.ToString(CultureInfo.InvariantCulture)
        );
    }
}
