namespace Savor22b.States;

using Bencodex.Types;


public class RootState : State
{
    public InventoryState InventoryState { get; private set; }

    public VillageState? VillageState { get; private set; }


    public RootState()
    {
        InventoryState = new InventoryState();
        VillageState = null;
    }

    public RootState(InventoryState inventoryState)
    {
        InventoryState = inventoryState;
        VillageState = null;
    }

    public RootState(InventoryState inventoryState, VillageState villageState)
    {
        InventoryState = inventoryState;
        VillageState = villageState;
    }

    public RootState(Bencodex.Types.Dictionary encoded)
    {
        if (encoded.ContainsKey((Text)nameof(InventoryState)))
        {
            InventoryState = new InventoryState((Bencodex.Types.Dictionary)encoded[nameof(InventoryState)]);
        }
        else
        {
            InventoryState = new InventoryState();
        }

        if (encoded.ContainsKey((Text)nameof(VillageState)))
        {
            VillageState = new VillageState((Bencodex.Types.Dictionary)encoded[nameof(VillageState)]);
        }
        else
        {
            VillageState = null;
        }
    }

    public void SetVillageState(VillageState villageState)
    {
        VillageState = villageState;
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(InventoryState), InventoryState.Serialize())
        };

        if (VillageState is not null)
        {
            pairs = pairs.Append(new KeyValuePair<IKey, IValue>((Text)nameof(VillageState), VillageState.Serialize())).ToArray();
        }


        return new Dictionary(pairs);
    }
}
