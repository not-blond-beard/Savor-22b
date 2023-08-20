namespace Savor22b.Util;

using System;

public static class MathUtil
{
    public static long ReduceByPercentage(long originalValue, int reductionPercentage)
    {
        if (reductionPercentage < 0 || reductionPercentage > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(reductionPercentage),
                "Reduction percentage must be between 0 and 100."
            );
        }

        return originalValue * (100 - reductionPercentage) / 100;
    }

    public static double CalculateEuclideanDistance(
        int originTargetX,
        int originTargetY,
        int targetX,
        int targetY
    )
    {
        double distance = Math.Sqrt(
            Math.Pow(targetX - originTargetX, 2) + Math.Pow(targetY - originTargetY, 2)
        );

        return distance;
    }
}
