namespace Savor22b.States;

using Bencodex.Types;

public class SeedState
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    public SeedState(int id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    public SeedState(Bencodex.Types.Dictionary encoded)
    {
        this.Id = (int)((Integer)encoded[(Text)"id"]).Value;
        this.Name = (string)((Text)encoded[(Text)"name"]).Value;
    }

    public Dictionary ToBencodex()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)"id", (Integer)this.Id),
            new KeyValuePair<IKey, IValue>((Text)"name", (Text)this.Name),
        };
        return new Dictionary(pairs);
    }
}
