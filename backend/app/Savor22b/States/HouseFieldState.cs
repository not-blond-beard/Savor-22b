namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class HouseFieldState : State
{
    public Guid InstalledSeedID { get; private set; }
    public Guid SeedID { get; private set; }
    public int? InstalledBlock { get; private set; }
    public int? TotalBlock { get; private set; }
    public int? LastWeedBlock { get; private set; }



    public HouseFieldState()
    {
        InstalledSeedID = Guid.Empty;
        SeedID = Guid.Empty;
    }

    public HouseFieldState(Guid? installedSeedID, Guid? seedID, int? installedBlock, int? totalBlock, int? lastWeedBlock)
    {
        InstalledSeedID = installedSeedID ?? Guid.Empty;
        SeedID = seedID ?? Guid.Empty;
        InstalledBlock = installedBlock;
        TotalBlock = totalBlock;
        LastWeedBlock = lastWeedBlock;
    }

    public HouseFieldState(Bencodex.Types.Dictionary encoded)
    {
        if (encoded.ContainsKey(nameof(InstalledSeedID)))
        {
            InstalledSeedID = encoded[nameof(InstalledSeedID)].ToGuid();
        }
        else
        {
            InstalledSeedID = Guid.Empty;
        }

        if (encoded.ContainsKey(nameof(SeedID)))
        {
            SeedID = encoded[nameof(SeedID)].ToGuid();
        }
        else
        {
            SeedID = Guid.Empty;
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

        if (InstalledSeedID != Guid.Empty)
        {
            pairs = pairs.Append(new KeyValuePair<IKey, IValue>((Text)nameof(InstalledSeedID), InstalledSeedID.Serialize())).ToArray();
        }

        if (SeedID != Guid.Empty)
        {
            pairs = pairs.Append(new KeyValuePair<IKey, IValue>((Text)nameof(SeedID), SeedID.Serialize())).ToArray();
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
