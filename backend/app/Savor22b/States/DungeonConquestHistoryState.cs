namespace Savor22b.States;

using Bencodex.Types;
using Libplanet;
using Libplanet.Headless.Extensions;

public class DungeonConquestHistoryState : State
{
    public long BlockIndex { get; private set; }
    public int DungeonId { get; private set; }
    public Address TargetUserAddress { get; private set; }
    public int DungeonConquestStatus { get; private set; }

    public DungeonConquestHistoryState(
        long blockIndex,
        int dungeonId,
        Address targetUserAddress,
        int dungeonConquestStatus
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
        TargetUserAddress = encoded[nameof(TargetUserAddress)].ToAddress();
        DungeonConquestStatus = encoded[nameof(DungeonConquestStatus)].ToInteger();
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(BlockIndex), BlockIndex.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(DungeonId), DungeonId.Serialize()),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(TargetUserAddress),
                TargetUserAddress.ToBencodex()
            ),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(DungeonConquestStatus),
                DungeonConquestStatus.Serialize()
            ),
        };

        return new Dictionary(pairs);
    }
}
