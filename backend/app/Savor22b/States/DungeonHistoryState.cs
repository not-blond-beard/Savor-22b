namespace Savor22b.States;

using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class DungeonHistoryState : State
{
    public long BlockIndex { get; private set; }
    public int DungeonId { get; private set; }
    public int DungeonClearStatus { get; private set; }
    public ImmutableList<int> DungeonClearRewardSeedIdList { get; private set; }

    public DungeonHistoryState(
        long blockIndex,
        int dungeonId,
        int dungeonClearStatus,
        ImmutableList<int> dungeonClearRewardSeedIdList
    )
    {
        BlockIndex = blockIndex;
        DungeonId = dungeonId;
        DungeonClearStatus = dungeonClearStatus;
        DungeonClearRewardSeedIdList = dungeonClearRewardSeedIdList;
    }

    public DungeonHistoryState(Dictionary encoded)
    {
        BlockIndex = encoded[nameof(BlockIndex)].ToLong();
        DungeonId = encoded[nameof(DungeonId)].ToInteger();
        DungeonClearStatus = encoded[nameof(DungeonClearStatus)].ToInteger();
        DungeonClearRewardSeedIdList = (
            (Bencodex.Types.List)encoded[nameof(DungeonClearRewardSeedIdList)]
        )
            .Select(e => e.ToInteger())
            .ToImmutableList();
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(BlockIndex), BlockIndex.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(DungeonId), DungeonId.Serialize()),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(DungeonClearStatus),
                DungeonClearStatus.Serialize()
            ),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(DungeonClearRewardSeedIdList),
                new List(DungeonClearRewardSeedIdList.Select(e => e.Serialize()))
            ),
        };

        return new Dictionary(pairs);
    }
}
