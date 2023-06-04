namespace Savor22b.States;

using System.Collections.Immutable;
using Bencodex.Types;

public class InventoryState : State
{
    public ImmutableList<SeedState> SeedStateList { get; private set; }
    public ImmutableList<RefrigeratorState> RefrigeratorStateList { get; private set; }

    public InventoryState()
    {
        this.SeedStateList = ImmutableList<SeedState>.Empty;
        this.RefrigeratorStateList = ImmutableList<RefrigeratorState>.Empty;
    }

    public InventoryState(ImmutableList<SeedState> seedStateList, ImmutableList<RefrigeratorState>? refrigeratorStateList)
    {
        this.SeedStateList = seedStateList;
        this.RefrigeratorStateList = refrigeratorStateList ?? ImmutableList<RefrigeratorState>.Empty;
    }

    public InventoryState(Bencodex.Types.Dictionary encoded)
    {
        if (encoded.TryGetValue((Text)"seedStateList", out var seedStateList))
        {
            this.SeedStateList = ((Bencodex.Types.List)seedStateList)
                .Select(element => new SeedState((Bencodex.Types.Dictionary)element))
                .ToImmutableList();
        }
        else
        {
            this.SeedStateList = ImmutableList<SeedState>.Empty;
        }

        if (encoded.TryGetValue((Text)"refrigeratorStateList", out var refrigeratorStateList))
        {
            this.RefrigeratorStateList = ((Bencodex.Types.List)refrigeratorStateList)
                .Select(element => new RefrigeratorState((Bencodex.Types.Dictionary)element))
                .ToImmutableList();
        }
        else
        {
            this.RefrigeratorStateList = ImmutableList<RefrigeratorState>.Empty;
        }
    }

    private IValue ImmutableListToBencodex(ImmutableList<SeedState> seedStateList)
    {
        var list = new Bencodex.Types.List();
        foreach (var seedState in seedStateList)
        {
            list = list.Add(seedState.Serialize());
        }
        return list;
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)"seedStateList",
                new Bencodex.Types.List(this.SeedStateList.Select(element => element.Serialize()))),
            new KeyValuePair<IKey, IValue>((Text)"refrigeratorStateList",
                new Bencodex.Types.List(this.RefrigeratorStateList.Select(element => element.Serialize()))),
        };
        return new Dictionary(pairs);
    }

    public InventoryState RemoveSeed(int seedStateId)
    {
        var seedStateList = this.SeedStateList.RemoveAll(seedState => seedState.Id == seedStateId);
        return new InventoryState(seedStateList, this.RefrigeratorStateList);
    }

    public InventoryState AddSeed(SeedState seedState)
    {
        var seedStateList = this.SeedStateList.Add(seedState);
        return new InventoryState(seedStateList, this.RefrigeratorStateList);
    }

    public InventoryState AddRefrigeratorItem(RefrigeratorState item)
    {
        var refrigeratorStateList = this.RefrigeratorStateList.Add(item);
        return new InventoryState(this.SeedStateList, refrigeratorStateList);
    }
    public int NextRefrigeratorId
    {
        get
        {
            if (this.RefrigeratorStateList.Count == 0)
            {
                return 0;
            }
            return this.RefrigeratorStateList[this.RefrigeratorStateList.Count - 1].Id + 1;
        }
    }

    public int NextSeedId
    {
        get
        {
            if (this.SeedStateList.Count == 0)
            {
                return 0;
            }
            return this.SeedStateList[this.SeedStateList.Count - 1].Id + 1;
        }
    }
}
