namespace Savor22b.States;

using Bencodex.Types;
using Libplanet;
using Libplanet.Headless.Extensions;

public class DungeonConquestHistoryState : State
{
    public long BlockIndex { get; private set; }
    public int DungeonId { get; private set; }
    public int DungeonConquestStatus { get; private set; }
    public Address? TargetUserAddress { get; private set; }

    public DungeonConquestHistoryState(
        long blockIndex,
        int dungeonId,
        int dungeonConquestStatus,
        Address? targetUserAddress = null
    )
    {
        BlockIndex = blockIndex;
        DungeonId = dungeonId;
        TargetUserAddress = targetUserAddress;
        DungeonConquestStatus = dungeonConquestStatus;
    }

    public DungeonConquestHistoryState(Dictionary encoded)
    {
        BlockIndex = encoded[nameof(BlockIndex)].ToLong();
        DungeonId = encoded[nameof(DungeonId)].ToInteger();
        DungeonConquestStatus = encoded[nameof(DungeonConquestStatus)].ToInteger();

        if (encoded.TryGetValue((Text)nameof(TargetUserAddress), out var targetUserAddress))
        {
            TargetUserAddress = targetUserAddress.ToAddress();
        }
        else
        {
            TargetUserAddress = null;
        }
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(BlockIndex), BlockIndex.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(DungeonId), DungeonId.Serialize()),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(DungeonConquestStatus),
                DungeonConquestStatus.Serialize()
            ),
        };

        if (TargetUserAddress is Address targetUserAddress)
        {
            pairs = pairs
                .Append(
                    new KeyValuePair<IKey, IValue>(
                        (Text)nameof(TargetUserAddress),
                        targetUserAddress.ToBencodex()
                    )
                )
                .ToArray();
        }

        return new Dictionary(pairs);
    }
}
