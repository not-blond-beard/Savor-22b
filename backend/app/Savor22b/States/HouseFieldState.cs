namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class HouseFieldState : State
{
    public Guid InstalledSeedGuid { get; private set; }
    public int SeedID { get; private set; }
    public long InstalledBlock { get; private set; }
    public int TotalBlock { get; private set; }
    public long? LastWeedBlock { get; private set; }


    public HouseFieldState()
    {
        InstalledSeedGuid = Guid.Empty;
    }

    public HouseFieldState(Guid installedSeedGuid, int seedID, long installedBlock, int totalBlock, long? lastWeedBlock)
    {
        InstalledSeedGuid = installedSeedGuid;
        SeedID = seedID;
        InstalledBlock = installedBlock;
        TotalBlock = totalBlock;
        LastWeedBlock = lastWeedBlock ?? null;
    }

    public HouseFieldState(Dictionary encoded)
    {
        InstalledSeedGuid = encoded[nameof(InstalledSeedGuid)].ToGuid();
        SeedID = encoded[nameof(SeedID)].ToInteger();
        InstalledBlock = encoded[nameof(InstalledBlock)].ToLong();
        TotalBlock = encoded[nameof(TotalBlock)].ToInteger();
        LastWeedBlock = encoded.TryGetValue((Text)nameof(LastWeedBlock), out var lastWeedBlock) ? lastWeedBlock.ToLong() : null;
    }


    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(InstalledSeedGuid), InstalledSeedGuid.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(SeedID), SeedID.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(InstalledBlock), InstalledBlock.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(TotalBlock), TotalBlock.Serialize())
        };

        if (LastWeedBlock is not null)
        {
            pairs.Append(new KeyValuePair<IKey, IValue>((Text)nameof(LastWeedBlock), ((long)LastWeedBlock).Serialize()));
        }

        return new Dictionary(pairs);
    }

    public bool IsHarvestable(long currentBlockIndex)
    {
        return InstalledBlock + TotalBlock <= currentBlockIndex;
    }
}
