namespace Savor22b.States;

using Bencodex.Types;

public class RootState : State
{
    public InventoryState InventoryState { get; private set; }
    public UserDungeonState UserDungeonState { get; private set; }

    public RelocationState? RelocationState { get; private set; }

    public VillageState? VillageState { get; private set; }

    public RootState()
    {
        InventoryState = new InventoryState();
        UserDungeonState = new UserDungeonState();
        VillageState = null;
        RelocationState = null;
    }

    public RootState(
        InventoryState inventoryState,
        UserDungeonState dungeonState,
        VillageState? villageState = null,
        RelocationState? relocationState = null
    )
    {
        InventoryState = inventoryState ?? new InventoryState();
        UserDungeonState = dungeonState ?? new UserDungeonState();
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

        if (encoded.ContainsKey((Text)nameof(UserDungeonState)))
        {
            UserDungeonState = new UserDungeonState(
                (Bencodex.Types.Dictionary)encoded[nameof(UserDungeonState)]
            );
        }
        else
        {
            UserDungeonState = new UserDungeonState();
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

    public void SetUserDungeonState(UserDungeonState userDungeonState)
    {
        UserDungeonState = userDungeonState;
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(InventoryState),
                InventoryState.Serialize()
            ),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(UserDungeonState),
                UserDungeonState.Serialize()
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
