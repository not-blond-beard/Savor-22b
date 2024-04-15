namespace Savor22b.States;

using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class DungeonConquestPeriodicRewardHistoryState : State
{
    public DungeonConquestPeriodicRewardHistoryState(
        long blockIndex,
        int dungeonId,
        ImmutableList<int> rewardSeedIdList
    )
    {
        BlockIndex = blockIndex;
        DungeonId = dungeonId;
        RewardSeedIdList = rewardSeedIdList;
    }

    public DungeonConquestPeriodicRewardHistoryState(Dictionary encoded)
    {
        BlockIndex = encoded[nameof(BlockIndex)].ToLong();
        DungeonId = encoded[nameof(DungeonId)].ToInteger();
        RewardSeedIdList = ((List)encoded[nameof(RewardSeedIdList)])
            .Select(e => e.ToInteger())
            .ToImmutableList();
    }

    public long BlockIndex { get; private set; }
    public int DungeonId { get; private set; }
    public ImmutableList<int> RewardSeedIdList { get; private set; }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(BlockIndex), BlockIndex.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(DungeonId), DungeonId.Serialize()),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(RewardSeedIdList),
                new List(RewardSeedIdList.Select(e => e.Serialize()))
            ),
        };

        return new Dictionary(pairs);
    }
}
