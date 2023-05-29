namespace Savor22b.States;

using System.Collections.Immutable;
using Bencodex.Types;

public class InventoryState
{
    public ImmutableList<SeedState> SeedStateList { get; private set; }

    public InventoryState()
    {
        this.SeedStateList = ImmutableList<SeedState>.Empty;
    }

    public InventoryState(ImmutableList<SeedState> seedStateList)
    {
        this.SeedStateList = seedStateList;
    }

    public InventoryState(Bencodex.Types.Dictionary encoded)
    {
        this.SeedStateList = ((Bencodex.Types.List)encoded[(Text)"seedStateList"])
            .Select(element => new SeedState((Bencodex.Types.Dictionary)element))
            .ToImmutableList();
    }

    private IValue ImmutableListToBencodex(ImmutableList<SeedState> seedStateList)
    {
        var list = new Bencodex.Types.List();
        foreach (var seedState in seedStateList)
        {
            list = list.Add(seedState.ToBencodex());
        }
        return list;
    }

    public Dictionary ToBencodex()
    {
        var seedStateList = this.SeedStateList;
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)"seedStateList", ImmutableListToBencodex(seedStateList)),
        };
        return new Dictionary(pairs);
    }

    public InventoryState AddSeed(SeedState seedState)
    {
        var seedStateList = this.SeedStateList.Add(seedState);
        return new InventoryState(seedStateList);
    }

    public int nextSeedId => this.SeedStateList.Count + 1;
}
