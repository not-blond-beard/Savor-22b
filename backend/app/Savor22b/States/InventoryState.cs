namespace Savor22b.States;

using System.Collections.Immutable;
using Bencodex.Types;

public class InventoryState : State
{
    public ImmutableList<SeedState> SeedStateList { get; private set; }
    public ImmutableList<RefrigeratorState> RefrigeratorStateList { get; private set; }
    public ImmutableList<CookingEquipmentState> CookingEquipmentStateList { get; private set; }

    public InventoryState()
    {
        SeedStateList = ImmutableList<SeedState>.Empty;
        RefrigeratorStateList = ImmutableList<RefrigeratorState>.Empty;
        CookingEquipmentStateList = ImmutableList<CookingEquipmentState>.Empty;
    }

    public InventoryState(ImmutableList<SeedState> seedStateList, ImmutableList<RefrigeratorState>? refrigeratorStateList, ImmutableList<CookingEquipmentState>? cookingEquipmentStateList)
    {
        SeedStateList = seedStateList;
        RefrigeratorStateList = refrigeratorStateList ?? ImmutableList<RefrigeratorState>.Empty;
        CookingEquipmentStateList = cookingEquipmentStateList ?? ImmutableList<CookingEquipmentState>.Empty;
    }

    public InventoryState(Bencodex.Types.Dictionary encoded)
    {
        if (encoded.TryGetValue((Text)nameof(SeedStateList), out var seedStateList))
        {
            SeedStateList = ((Bencodex.Types.List)seedStateList)
                .Select(element => new SeedState((Bencodex.Types.Dictionary)element))
                .ToImmutableList();
        }
        else
        {
            SeedStateList = ImmutableList<SeedState>.Empty;
        }

        if (encoded.TryGetValue((Text)nameof(RefrigeratorStateList), out var refrigeratorStateList))
        {
            RefrigeratorStateList = ((Bencodex.Types.List)refrigeratorStateList)
                .Select(element => new RefrigeratorState((Bencodex.Types.Dictionary)element))
                .ToImmutableList();
        }
        else
        {
            RefrigeratorStateList = ImmutableList<RefrigeratorState>.Empty;
        }

        if (encoded.TryGetValue((Text)nameof(CookingEquipmentStateList), out var cookingEquipmentStateList))
        {
            CookingEquipmentStateList = ((Bencodex.Types.List)cookingEquipmentStateList)
                .Select(element => new CookingEquipmentState((Bencodex.Types.Dictionary)element))
                .ToImmutableList();
        }
        else
        {
            CookingEquipmentStateList = ImmutableList<CookingEquipmentState>.Empty;
        }
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(SeedStateList),
                new Bencodex.Types.List(SeedStateList.Select(element => element.Serialize()))),
            new KeyValuePair<IKey, IValue>((Text)nameof(RefrigeratorStateList),
                new Bencodex.Types.List(RefrigeratorStateList.Select(element => element.Serialize()))),
            new KeyValuePair<IKey, IValue>((Text)nameof(CookingEquipmentStateList),
                new Bencodex.Types.List(CookingEquipmentStateList.Select(element => element.Serialize()))),
        };
        return new Dictionary(pairs);
    }

    public InventoryState RemoveSeed(Guid seedStateID)
    {
        var seedStateList = SeedStateList.RemoveAll(seedState => seedState.StateID == seedStateID);
        return new InventoryState(seedStateList, RefrigeratorStateList, CookingEquipmentStateList);
    }

    public InventoryState AddSeed(SeedState seedState)
    {
        var seedStateList = SeedStateList.Add(seedState);
        return new InventoryState(seedStateList, RefrigeratorStateList, CookingEquipmentStateList);
    }

    public InventoryState AddRefrigeratorItem(RefrigeratorState item)
    {
        var refrigeratorStateList = RefrigeratorStateList.Add(item);
        return new InventoryState(SeedStateList, refrigeratorStateList, CookingEquipmentStateList);
    }

    public InventoryState RemoveRefrigeratorItem(Guid stateID)
    {
        var stateList = RefrigeratorStateList.RemoveAll(state => state.StateID == stateID);
        return new InventoryState(SeedStateList, stateList, CookingEquipmentStateList);
    }

    public InventoryState AddCookingEquipmentItem(CookingEquipmentState item)
    {
        var cookingEquipmentStateList = CookingEquipmentStateList.Add(item);
        return new InventoryState(SeedStateList, RefrigeratorStateList, cookingEquipmentStateList);
    }

    public InventoryState RemoveCookingEquipmentItem(Guid stateID)
    {
        var stateList = CookingEquipmentStateList.RemoveAll(state => state.StateID == stateID);
        return new InventoryState(SeedStateList, RefrigeratorStateList, stateList);
    }
}
