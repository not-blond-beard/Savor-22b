namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class HouseFieldState : State
{
    public static readonly int WeedRemovalPercentage = 30;
    public static readonly int WeedRemovalImpactPercentage = 5;
    public Guid InstalledSeedGuid { get; private set; }
    public int SeedID { get; private set; }
    public long InstalledBlock { get; private set; }
    public int TotalBlock { get; private set; }
    public int WeedRemovalCount { get; private set; }
    public long? LastWeedBlock { get; private set; }


    public HouseFieldState()
    {
        InstalledSeedGuid = Guid.Empty;
    }

    public HouseFieldState(Guid installedSeedGuid, int seedID, long installedBlock, int totalBlock)
    {
        InstalledSeedGuid = installedSeedGuid;
        SeedID = seedID;
        InstalledBlock = installedBlock;
        TotalBlock = totalBlock;
        LastWeedBlock = null;
        WeedRemovalCount = 0;
    }

    public HouseFieldState(Guid installedSeedGuid, int seedID, long installedBlock, int totalBlock, long? lastWeedBlock, int? weedRemovalCount)
    {
        InstalledSeedGuid = installedSeedGuid;
        SeedID = seedID;
        InstalledBlock = installedBlock;
        TotalBlock = totalBlock;
        LastWeedBlock = lastWeedBlock ?? null;
        WeedRemovalCount = weedRemovalCount ?? 0;
    }

    public HouseFieldState(Dictionary encoded)
    {
        InstalledSeedGuid = encoded[nameof(InstalledSeedGuid)].ToGuid();
        SeedID = encoded[nameof(SeedID)].ToInteger();
        InstalledBlock = encoded[nameof(InstalledBlock)].ToLong();
        TotalBlock = encoded[nameof(TotalBlock)].ToInteger();
        WeedRemovalCount = encoded[nameof(WeedRemovalCount)].ToInteger();
        LastWeedBlock = encoded.TryGetValue((Text)nameof(LastWeedBlock), out var lastWeedBlock) ? lastWeedBlock.ToLong() : null;
    }

    public bool RemovalWeed(long blockIndex)
    {
        if (WeedRemovalAble(blockIndex) is false)
        {
            return false;
        }

        WeedRemovalCount = WeedRemovalCount + 1;
        LastWeedBlock = blockIndex;

        return true;
    }

    public bool WeedRemovalAble(long blockIndex)
    {
        long targetBlockIndex = LastWeedBlock ?? InstalledBlock;

        if (blockIndex >= targetBlockIndex + WeedRemovalBlockUnit())
        {
            return true;
        }

        return false;
    }

    public int WeedRemovalBlockUnit()
    {
        int weedRemovalBlockUnit = (int)Math.Ceiling(TotalBlock * WeedRemovalPercentage / 100.0);

        return weedRemovalBlockUnit;
    }

    public int WeedRemovalImpactBlock()
    {
        int weedRemovalImpact = (int)Math.Ceiling(TotalBlock * WeedRemovalImpactPercentage / 100.0);

        return weedRemovalImpact;
    }


    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(InstalledSeedGuid), InstalledSeedGuid.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(SeedID), SeedID.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(InstalledBlock), InstalledBlock.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(TotalBlock), TotalBlock.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(WeedRemovalCount), WeedRemovalCount.Serialize()),
        };

        if (LastWeedBlock is not null)
        {
            pairs = pairs.Append(new KeyValuePair<IKey, IValue>((Text)nameof(LastWeedBlock), ((long)LastWeedBlock).Serialize())).ToArray();
        }

        return new Dictionary(pairs);
    }

    public bool IsHarvestable(long currentBlockIndex)
    {
        return InstalledBlock + TotalBlock <= currentBlockIndex;
    }
}
