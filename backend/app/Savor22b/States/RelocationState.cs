namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class RelocationState : State
{
    public RelocationState(
        long startedBlock,
        int durationBlock,
        int targetVillageID,
        int targetPositionX,
        int targetPositionY
    )
    {
        StartedBlock = startedBlock;
        DurationBlock = durationBlock;
        TargetVillageID = targetVillageID;
        TargetPositionX = targetPositionX;
        TargetPositionY = targetPositionY;
    }

    public RelocationState(Bencodex.Types.Dictionary encoded)
    {
        StartedBlock = encoded[nameof(StartedBlock)].ToLong();
        DurationBlock = encoded[nameof(DurationBlock)].ToInteger();
        TargetVillageID = encoded[nameof(TargetVillageID)].ToInteger();
        TargetPositionX = encoded[nameof(TargetPositionX)].ToInteger();
        TargetPositionY = encoded[nameof(TargetPositionY)].ToInteger();
    }

    public long StartedBlock { get; private set; }

    public int DurationBlock { get; private set; }

    public int TargetVillageID { get; private set; }

    public int TargetPositionX { get; private set; }

    public int TargetPositionY { get; private set; }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(StartedBlock), StartedBlock.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(DurationBlock), DurationBlock.Serialize()),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(TargetVillageID),
                TargetVillageID.Serialize()
            ),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(TargetPositionX),
                TargetPositionX.Serialize()
            ),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(TargetPositionY),
                TargetPositionY.Serialize()
            ),
        };
        return new Dictionary(pairs);
    }

    public bool IsRelocationInProgress(long currentBlock)
    {
        return StartedBlock + DurationBlock > currentBlock;
    }
}
