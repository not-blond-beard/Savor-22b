namespace Savor22b.States;

using Bencodex.Types;

public class SeedState : State
{
    public int Id { get; private set; }
    public int SeedID { get; private set; }

    public SeedState(int id, int seedId)
    {
        this.Id = id;
        this.SeedID = seedId;
    }

    public SeedState(Bencodex.Types.Dictionary encoded)
    {
        this.Id = (int)((Integer)encoded[(Text)"id"]).Value;
        this.SeedID = (int)((Integer)encoded[(Text)"seedId"]).Value;
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)"id", (Integer)this.Id),
            new KeyValuePair<IKey, IValue>((Text)"seedId", (Integer)this.SeedID),
        };
        return new Dictionary(pairs);
    }
}
