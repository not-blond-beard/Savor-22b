namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class KitchenEquipmentState : State
{
    public Guid StateID { get; private set; }
    public int KitchenEquipmentID { get; private set; }
    public bool IsInUse { get; private set; }

    public KitchenEquipmentState(Guid stateID, int kitchenEquipmentID, bool isInUse)
    {
        StateID = stateID;
        KitchenEquipmentID = kitchenEquipmentID;
        IsInUse = isInUse;
    }

    public KitchenEquipmentState(Guid stateID, int kitchenEquipmentID)
    {
        StateID = stateID;
        KitchenEquipmentID = kitchenEquipmentID;
        IsInUse = false;
    }

    public KitchenEquipmentState(Bencodex.Types.Dictionary encoded)
    {
        StateID = encoded[nameof(StateID)].ToGuid();
        KitchenEquipmentID = (int)((Integer)encoded[nameof(KitchenEquipmentID)]).Value;
        IsInUse = (bool)((Boolean)encoded[nameof(IsInUse)]).Value;
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(StateID), StateID.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(KitchenEquipmentID), (Integer)this.KitchenEquipmentID),
            new KeyValuePair<IKey, IValue>((Text)nameof(IsInUse), (Boolean)this.IsInUse),
        };
        return new Dictionary(pairs);
    }
}
