namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class ItemState : State
{
    public Guid StateID { get; private set; }
    public int ItemID { get; private set; }

    public ItemState(Guid stateID, int itemID)
    {
        StateID = stateID;
        ItemID = itemID;
    }

    public ItemState(Bencodex.Types.Dictionary encoded)
    {
        StateID = encoded[nameof(StateID)].ToGuid();
        ItemID = (int)((Integer)encoded[nameof(ItemID)]).Value;
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(StateID), StateID.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(ItemID), (Integer)this.ItemID),
        };
        return new Dictionary(pairs);
    }
}
