namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class SeedState : State
{
    public Guid Id { get; private set; }
    public int SeedID { get; private set; }

    public SeedState(Guid id, int seedId)
    {
        Id = id;
        SeedID = seedId;
    }

    public SeedState(Bencodex.Types.Dictionary encoded)
    {
        Id = encoded["id"].ToGuid();
        SeedID = (int)((Integer)encoded[(Text)"seedId"]).Value;
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)"id", this.Id.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)"seedId", (Integer)this.SeedID),
        };
        return new Dictionary(pairs);
    }
}
