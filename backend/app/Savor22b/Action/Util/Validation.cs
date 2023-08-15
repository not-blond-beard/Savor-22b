namespace Savor22b.Action.Util;

using Savor22b.States;
using Savor22b.Action.Exceptions;

public static class Validation
{
    public static void CheckPlacedHouse(RootState rootState)
    {
        if (rootState.VillageState is null)
        {
            throw new InvalidVillageStateException("VillageState is null");
        }
    }
}
