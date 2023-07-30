namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class HouseFieldState : State
{
    public Guid StateID { get; private set; }
    public Guid InstalledSeedID { get; private set; }
    public Guid SeedID { get; private set; }
    public int? InstalledBlock { get; private set; }
    public int? TotalBlock { get; private set; }
    public int? LastWeedBlock { get; private set; }



    public HouseFieldState()
    {
        StateID = Guid.NewGuid();
        InstalledSeedID = Guid.Empty;
        SeedID = Guid.Empty;
    }

    public HouseFieldState(Guid stateID, Guid? installedSeedID, Guid? seedID, int? installedBlock, int? totalBlock, int? lastWeedBlock)
    {
        StateID = stateID;
        InstalledSeedID = installedSeedID ?? Guid.Empty;
        SeedID = seedID ?? Guid.Empty;
        InstalledBlock = installedBlock;
        TotalBlock = totalBlock;
        LastWeedBlock = lastWeedBlock;
    }

    public HouseFieldState(Bencodex.Types.Dictionary encoded)
    {
        StateID = encoded[nameof(StateID)] == null ? Guid.Empty : encoded[nameof(StateID)].ToGuid();
        InstalledSeedID = encoded[nameof(InstalledSeedID)] == null ? Guid.Empty : encoded[nameof(InstalledSeedID)].ToGuid();
        SeedID = encoded[nameof(SeedID)] == null ? Guid.Empty : encoded[nameof(SeedID)].ToGuid();
        InstalledBlock = encoded[nameof(InstalledBlock)] == null ? null : encoded[nameof(InstalledBlock)].ToInteger();
        TotalBlock = encoded[nameof(TotalBlock)] == null ? null : encoded[nameof(TotalBlock)].ToInteger();
        LastWeedBlock = encoded[nameof(LastWeedBlock)] == null ? null : encoded[nameof(LastWeedBlock)].ToInteger();
    }


    public IValue Serialize()
    {

        var pairs = new[]
        {
             new KeyValuePair<IKey, IValue>((Text)nameof(StateID), StateID.Serialize()),
        };

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
