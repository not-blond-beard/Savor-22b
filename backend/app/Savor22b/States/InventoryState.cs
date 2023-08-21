namespace Savor22b.States;

using System.Collections.Immutable;
using Bencodex.Types;

public class InventoryState : State
{
    public ImmutableList<SeedState> SeedStateList { get; private set; }
    public ImmutableList<RefrigeratorState> RefrigeratorStateList { get; private set; }
    public ImmutableList<KitchenEquipmentState> KitchenEquipmentStateList { get; private set; }
    public ImmutableList<ItemState> ItemStateList { get; private set; }

    public InventoryState()
    {
        SeedStateList = ImmutableList<SeedState>.Empty;
        RefrigeratorStateList = ImmutableList<RefrigeratorState>.Empty;
        KitchenEquipmentStateList = ImmutableList<KitchenEquipmentState>.Empty;
        ItemStateList = ImmutableList<ItemState>.Empty;
    }

    public InventoryState(
        ImmutableList<SeedState> seedStateList,
        ImmutableList<RefrigeratorState>? refrigeratorStateList,
        ImmutableList<KitchenEquipmentState>? kitchenEquipmentStateList,
        ImmutableList<ItemState>? itemStateList
    )
    {
        SeedStateList = seedStateList;
        RefrigeratorStateList = refrigeratorStateList ?? ImmutableList<RefrigeratorState>.Empty;
        KitchenEquipmentStateList =
            kitchenEquipmentStateList ?? ImmutableList<KitchenEquipmentState>.Empty;
        ItemStateList = itemStateList ?? ImmutableList<ItemState>.Empty;
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

        if (
            encoded.TryGetValue(
                (Text)nameof(KitchenEquipmentStateList),
                out var kitchenEquipmentStateList
            )
        )
        {
            KitchenEquipmentStateList = ((Bencodex.Types.List)kitchenEquipmentStateList)
                .Select(element => new KitchenEquipmentState((Bencodex.Types.Dictionary)element))
                .ToImmutableList();
        }
        else
        {
            KitchenEquipmentStateList = ImmutableList<KitchenEquipmentState>.Empty;
        }

        if (encoded.TryGetValue((Text)nameof(ItemStateList), out var itemStateList))
        {
            ItemStateList = ((Bencodex.Types.List)itemStateList)
                .Select(element => new ItemState((Bencodex.Types.Dictionary)element))
                .ToImmutableList();
        }
        else
        {
            ItemStateList = ImmutableList<ItemState>.Empty;
        }
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(SeedStateList),
                new Bencodex.Types.List(SeedStateList.Select(element => element.Serialize()))
            ),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(RefrigeratorStateList),
                new Bencodex.Types.List(
                    RefrigeratorStateList.Select(element => element.Serialize())
                )
            ),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(KitchenEquipmentStateList),
                new Bencodex.Types.List(
                    KitchenEquipmentStateList.Select(element => element.Serialize())
                )
            ),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(ItemStateList),
                new Bencodex.Types.List(ItemStateList.Select(element => element.Serialize()))
            ),
        };
        return new Dictionary(pairs);
    }

    public InventoryState RemoveSeed(Guid seedStateID)
    {
        var seedStateList = SeedStateList.RemoveAll(seedState => seedState.StateID == seedStateID);
        return new InventoryState(
            seedStateList,
            RefrigeratorStateList,
            KitchenEquipmentStateList,
            ItemStateList
        );
    }

    public InventoryState AddSeed(SeedState seedState)
    {
        var seedStateList = SeedStateList.Add(seedState);
        return new InventoryState(
            seedStateList,
            RefrigeratorStateList,
            KitchenEquipmentStateList,
            ItemStateList
        );
    }

    public InventoryState AddRefrigeratorItem(RefrigeratorState item)
    {
        var refrigeratorStateList = RefrigeratorStateList.Add(item);
        return new InventoryState(
            SeedStateList,
            refrigeratorStateList,
            KitchenEquipmentStateList,
            ItemStateList
        );
    }

    public InventoryState RemoveRefrigeratorItem(Guid stateID)
    {
        var stateList = RefrigeratorStateList.RemoveAll(state => state.StateID == stateID);
        return new InventoryState(
            SeedStateList,
            stateList,
            KitchenEquipmentStateList,
            ItemStateList
        );
    }

    public RefrigeratorState GetRefrigeratorItem(Guid stateID)
    {
        return RefrigeratorStateList.Find(r => r.StateID == stateID);
    }

    public InventoryState AddKitchenEquipmentItem(KitchenEquipmentState item)
    {
        var kitchenEquipmentStateList = KitchenEquipmentStateList.Add(item);
        return new InventoryState(
            SeedStateList,
            RefrigeratorStateList,
            kitchenEquipmentStateList,
            ItemStateList
        );
    }

    public InventoryState RemoveKitchenEquipmentItem(Guid stateID)
    {
        var stateList = KitchenEquipmentStateList.RemoveAll(state => state.StateID == stateID);
        return new InventoryState(SeedStateList, RefrigeratorStateList, stateList, ItemStateList);
    }

    public InventoryState AddItem(ItemState item)
    {
        var itemStateList = ItemStateList.Add(item);
        return new InventoryState(
            SeedStateList,
            RefrigeratorStateList,
            KitchenEquipmentStateList,
            itemStateList
        );
    }

    public InventoryState RemoveItem(Guid stateID)
    {
        var stateList = ItemStateList.RemoveAll(state => state.StateID == stateID);
        return new InventoryState(
            SeedStateList,
            RefrigeratorStateList,
            KitchenEquipmentStateList,
            stateList
        );
    }

    public SeedState? GetSeedState(Guid stateID)
    {
        return SeedStateList.Find(seedState => seedState.StateID == stateID);
    }

    public KitchenEquipmentState? GetKitchenEquipmentState(Guid stateID)
    {
        return KitchenEquipmentStateList.Find(k => k.StateID == stateID);
    }
}
