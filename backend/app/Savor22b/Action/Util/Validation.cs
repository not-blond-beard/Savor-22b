namespace Savor22b.Action.Util;

using Savor22b.States;
using Savor22b.Action.Exceptions;
using Savor22b.Model;

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

    public static Village GetVillage(int villageID)
    {
        var village = CsvDataHelper.GetVillageByID(villageID);

        if (village == null)
        {
            throw new InvalidVillageException("Invalid village ID");
        }

        return village;
    }

    public static void EnsureDungeonExist(int dungeonId)
    {
        Dungeon? dungeon = CsvDataHelper.GetDungeonById(dungeonId);

        if (dungeon == null)
        {
            throw new InvalidDungeonException($"Invalid dungeon ID: {dungeonId}");
        }
    }
}
