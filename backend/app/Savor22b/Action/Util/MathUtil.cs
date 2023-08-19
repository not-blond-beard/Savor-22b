namespace Savor22b.Action.Util;

public static class MathUtil
{
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
