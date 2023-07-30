namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class HouseFieldState : State
{
    public Guid InstalledSeedGuid { get; private set; }
    public int? SeedID { get; private set; }
    public int? InstalledBlock { get; private set; }
    public int? TotalBlock { get; private set; }
    public int? LastWeedBlock { get; private set; }



    public HouseFieldState()
    {
        InstalledSeedGuid = Guid.Empty;
    }

    public HouseFieldState(Guid? installedSeedGuid, int? seedID, int? installedBlock, int? totalBlock, int? lastWeedBlock)
    {
        InstalledSeedGuid = installedSeedGuid ?? Guid.Empty;
        SeedID = seedID;
        InstalledBlock = installedBlock;
        TotalBlock = totalBlock;
        LastWeedBlock = lastWeedBlock;
    }

    public HouseFieldState(Bencodex.Types.Dictionary encoded)
    {
        if (encoded.ContainsKey(nameof(InstalledSeedGuid)))
        {
            InstalledSeedGuid = encoded[nameof(InstalledSeedGuid)].ToGuid();
        }
        else
        {
            InstalledSeedGuid = Guid.Empty;
        }

        if (encoded.ContainsKey(nameof(SeedID)))
        {
            SeedID = encoded[nameof(SeedID)].ToInteger();
        }
        else
        {
            SeedID = null;
        }

        if (encoded.ContainsKey(nameof(InstalledBlock)))
        {
            InstalledBlock = encoded[nameof(InstalledBlock)].ToInteger();
        }
        else
        {
            InstalledBlock = null;
        }

        if (encoded.ContainsKey(nameof(TotalBlock)))
        {
            TotalBlock = encoded[nameof(TotalBlock)].ToInteger();
        }
        else
        {
            TotalBlock = null;
        }

        if (encoded.ContainsKey(nameof(LastWeedBlock)))
        {
            LastWeedBlock = encoded[nameof(LastWeedBlock)].ToInteger();
        }
        else
        {
            LastWeedBlock = null;
        }
    }


    public IValue Serialize()
    {

        var pairs = new KeyValuePair<IKey, IValue>[] { };

        if (InstalledSeedGuid != Guid.Empty)
        {
            pairs = pairs.Append(new KeyValuePair<IKey, IValue>((Text)nameof(InstalledSeedGuid), InstalledSeedGuid.Serialize())).ToArray();
        }

        if (SeedID is not null)
        {
            pairs = pairs.Append(new KeyValuePair<IKey, IValue>((Text)nameof(SeedID), (Integer)SeedID)).ToArray();
        }

        if (InstalledBlock is not null)
        {
            pairs = pairs.Append(new KeyValuePair<IKey, IValue>((Text)nameof(InstalledBlock), (Integer)InstalledBlock)).ToArray();
        }

        if (TotalBlock is not null)
        {
            pairs = pairs.Append(new KeyValuePair<IKey, IValue>((Text)nameof(TotalBlock), (Integer)TotalBlock)).ToArray();
        }

        if (LastWeedBlock is not null)
        {
            pairs = pairs.Append(new KeyValuePair<IKey, IValue>((Text)nameof(LastWeedBlock), (Integer)LastWeedBlock)).ToArray();
        }


        return new Dictionary(pairs);
    }
}
