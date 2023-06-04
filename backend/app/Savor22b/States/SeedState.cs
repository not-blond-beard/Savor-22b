namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class SeedState : State
{
    public Guid StateID { get; private set; }
    public int SeedID { get; private set; }

    public SeedState(Guid stateID, int seedID)
    {
        StateID = stateID;
        SeedID = seedID;
    }

    public SeedState(Bencodex.Types.Dictionary encoded)
    {
        StateID = encoded[nameof(StateID)].ToGuid();
        SeedID = (int)((Integer)encoded[nameof(SeedID)]).Value;
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(StateID), StateID.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(SeedID), (Integer)this.SeedID),
        };
        return new Dictionary(pairs);
    }
}
