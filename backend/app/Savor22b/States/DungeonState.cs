namespace Savor22b.States;

using System.Collections.Immutable;
using Bencodex.Types;

public class DungeonState : State
{
    public static readonly int MaxDungeonKey = 5;
    public static readonly int DungeonKeyChargeIntervalBlock = 12;

    public ImmutableList<DungeonKeyHistory> DungeonKeyHistories { get; private set; }

    public DungeonState()
    {
        DungeonKeyHistories = ImmutableList<DungeonKeyHistory>.Empty;
    }

    public DungeonState(ImmutableList<DungeonKeyHistory> dungeonKeyHistories)
    {
        DungeonKeyHistories = dungeonKeyHistories;
    }

    public DungeonState(Dictionary encoded)
    {
        if (encoded.TryGetValue((Text)nameof(DungeonKeyHistories), out var dungeonKeyHistories))
        {
            DungeonKeyHistories = ((List)dungeonKeyHistories)
                .Select(element => new DungeonKeyHistory((Dictionary)element))
                .ToImmutableList();
        }
        else
        {
            DungeonKeyHistories = ImmutableList<DungeonKeyHistory>.Empty;
        }
    }

    public IValue Serialize()
    {
        return new Dictionary(new[]
        {
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(DungeonKeyHistories),
                new List(DungeonKeyHistories.Select(element => element.Serialize()))),
        });
    }

    public ImmutableList<DungeonKeyHistory> GetCurrentDungeonKeyHistories(long blockIndex)
    {
        var lowerBoundIndex = blockIndex - (MaxDungeonKey * DungeonKeyChargeIntervalBlock);
        var result = new List<DungeonKeyHistory>();

        for (int i = DungeonKeyHistories.Count - 1; i >= 0; i--)
        {
            var history = DungeonKeyHistories[i];

            if (history.BlockIndex > lowerBoundIndex && history.BlockIndex <= blockIndex)
            {
                result.Add(history);
            }
            else if (history.BlockIndex <= lowerBoundIndex)
            {
                break;
            }
        }

        return result.ToImmutableList();
    }
}
