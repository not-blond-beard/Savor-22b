namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class DungeonKeyHistory : State
{
    public long BlockIndex { get; private set; }
    public int DungeonId { get; private set; }

    public DungeonKeyHistory(long blockIndex, int dungeonId)
    {
        BlockIndex = blockIndex;
        DungeonId = dungeonId;
    }

    public DungeonKeyHistory(Dictionary encoded)
    {
        BlockIndex = encoded[nameof(BlockIndex)].ToLong();
        DungeonId = encoded[nameof(DungeonId)].ToInteger();
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(BlockIndex), BlockIndex.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(DungeonId), DungeonId.Serialize()),
        };

        return new Dictionary(pairs);
    }
}
