namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class CookingEquipmentState : State
{
    public Guid StateID { get; private set; }
    public int CookingEquipmentID { get; private set; }
    public bool IsInUse { get; private set; }

    public CookingEquipmentState(Guid stateID, int cookingEquipmentID, bool isInUse)
    {
        StateID = stateID;
        CookingEquipmentID = cookingEquipmentID;
        IsInUse = isInUse;
    }

    public CookingEquipmentState(Guid stateID, int cookingEquipmentID)
    {
        StateID = stateID;
        CookingEquipmentID = cookingEquipmentID;
        IsInUse = false;
    }

    public CookingEquipmentState(Bencodex.Types.Dictionary encoded)
    {
        StateID = encoded[nameof(StateID)].ToGuid();
        CookingEquipmentID = (int)((Integer)encoded[nameof(CookingEquipmentID)]).Value;
        IsInUse = (bool)((Boolean)encoded[nameof(IsInUse)]).Value;
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(StateID), StateID.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(CookingEquipmentID), (Integer)this.CookingEquipmentID),
            new KeyValuePair<IKey, IValue>((Text)nameof(IsInUse), (Boolean)this.IsInUse),
        };
        return new Dictionary(pairs);
    }
}
