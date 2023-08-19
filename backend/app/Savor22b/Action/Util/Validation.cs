namespace Savor22b.Action.Util;

using Savor22b.States;
using Savor22b.Action.Exceptions;

public static class Validation
{
    public static void EnsureVillageStateExists(RootState rootState)
    {
        if (rootState.VillageState is null)
        {
            throw new InvalidVillageStateException("VillageState is null");
        }
    }

    public static void EnsureReplaceInProgress(RootState rootState, long blockIndex)
    {
        if (
            rootState.RelocationState is not null
            && rootState.RelocationState.IsRelocationInProgress(blockIndex)
        )
        {
            throw new RelocationInProgressException("Relocation is in progress");
        }
    }
}
