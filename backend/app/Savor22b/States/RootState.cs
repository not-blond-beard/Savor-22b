namespace Savor22b.States;

using Bencodex.Types;

public class RootState : State
{
    public InventoryState InventoryState { get; private set; }

    public RelocationState? RelocationState { get; private set; }

    public VillageState? VillageState { get; private set; }

    public RootState()
    {
        InventoryState = new InventoryState();
        VillageState = null;
        RelocationState = null;
    }

    public RootState(InventoryState inventoryState)
    {
        InventoryState = inventoryState;
        VillageState = null;
        RelocationState = null;
    }

    public RootState(InventoryState inventoryState, VillageState villageState)
    {
        InventoryState = inventoryState;
        VillageState = villageState;
        RelocationState = null;
    }

    public RootState(
        InventoryState inventoryState,
        VillageState villageState,
        RelocationState relocationState
    )
    {
        InventoryState = inventoryState;
        VillageState = villageState;
        RelocationState = relocationState;
    }

    public RootState(Bencodex.Types.Dictionary encoded)
    {
        if (encoded.ContainsKey((Text)nameof(InventoryState)))
        {
            InventoryState = new InventoryState(
                (Bencodex.Types.Dictionary)encoded[nameof(InventoryState)]
            );
        }
        else
        {
            InventoryState = new InventoryState();
        }

        if (encoded.ContainsKey((Text)nameof(VillageState)))
        {
            VillageState = new VillageState(
                (Bencodex.Types.Dictionary)encoded[nameof(VillageState)]
            );
        }
        else
        {
            VillageState = null;
        }

        if (encoded.ContainsKey((Text)nameof(RelocationState)))
        {
            RelocationState = new RelocationState(
                (Bencodex.Types.Dictionary)encoded[nameof(RelocationState)]
            );
        }
        else
        {
            RelocationState = null;
        }
    }

    public void SetVillageState(VillageState villageState)
    {
        VillageState = villageState;
    }

    public void SetInventoryState(InventoryState inventoryState)
    {
        InventoryState = inventoryState;
    }

    public void SetRelocationState(RelocationState relocationState)
    {
        RelocationState = relocationState;
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(InventoryState),
                InventoryState.Serialize()
            ),
        };

        if (RelocationState is not null)
        {
            pairs = pairs
                .Append(
                    new KeyValuePair<IKey, IValue>(
                        (Text)nameof(RelocationState),
                        RelocationState.Serialize()
                    )
                )
                .ToArray();
        }

        if (VillageState is not null)
        {
            pairs = pairs
                .Append(
                    new KeyValuePair<IKey, IValue>(
                        (Text)nameof(VillageState),
                        VillageState.Serialize()
                    )
                )
                .ToArray();
        }

        return new Dictionary(pairs);
    }
}
