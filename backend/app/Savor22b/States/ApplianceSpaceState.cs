namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;
using Savor22b.Util;

public class ApplianceSpaceState : State
{
    public ApplianceSpaceState(int spaceNumber)
    {
        SpaceNumber = spaceNumber;
        InstalledKitchenEquipmentStateId = null;
        CookingDurationBlock = null;
        CookingStartedBlockIndex = null;
    }

    public ApplianceSpaceState(
        int spaceNumber,
        Guid? installedKitchenEquipmentStateId,
        long? cookingDurationBlock,
        long? cookingStartedBlockIndex
    )
    {
        SpaceNumber = spaceNumber;
        InstalledKitchenEquipmentStateId = installedKitchenEquipmentStateId;
        CookingDurationBlock = cookingDurationBlock;
        CookingStartedBlockIndex = cookingStartedBlockIndex;
    }

    public ApplianceSpaceState(Dictionary encoded)
    {
        SpaceNumber = encoded[nameof(SpaceNumber)].ToInteger();
        InstalledKitchenEquipmentStateId = encoded[
            nameof(InstalledKitchenEquipmentStateId)
        ].ToNullableGuid();
        CookingDurationBlock = encoded[nameof(CookingDurationBlock)].ToNullableLong();
        CookingStartedBlockIndex = encoded[nameof(CookingStartedBlockIndex)].ToNullableLong();
    }

    public int SpaceNumber { get; private set; }

    public Guid? InstalledKitchenEquipmentStateId { get; private set; }

    public long? CookingDurationBlock { get; private set; }

    public long? CookingStartedBlockIndex { get; private set; }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(SpaceNumber), SpaceNumber.Serialize()),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(InstalledKitchenEquipmentStateId),
                InstalledKitchenEquipmentStateId.Serialize()
            ),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(CookingDurationBlock),
                CookingDurationBlock.Serialize()
            ),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(CookingStartedBlockIndex),
                CookingStartedBlockIndex.Serialize()
            ),
        };
        return new Dictionary(pairs);
    }

    public ApplianceSpaceState InstallKitchenEquipment(Guid installedKitchenEquipmentStateId)
    {
        return new ApplianceSpaceState(SpaceNumber, installedKitchenEquipmentStateId, null, null);
    }

    public void UnInstallKitchenEquipment()
    {
        InstalledKitchenEquipmentStateId = null;
        CookingStartedBlockIndex = null;
        CookingDurationBlock = null;
    }

    public bool IsInUse(long currentBlockIndex)
    {
        return BlockUtil.CalculateIsInProgress(
            currentBlockIndex,
            CookingStartedBlockIndex ?? 0,
            CookingDurationBlock ?? 0
        );
    }

    public bool EquipmentIsPresent()
    {
        return InstalledKitchenEquipmentStateId is not null;
    }

    public void StartCooking(long currentBlockIndex, long cookingDurationBlock)
    {
        CookingStartedBlockIndex = currentBlockIndex;
        CookingDurationBlock = cookingDurationBlock;
    }

    public void StopCooking()
    {
        CookingStartedBlockIndex = null;
        CookingDurationBlock = null;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        ApplianceSpaceState other = (ApplianceSpaceState)obj;
        return SpaceNumber == other.SpaceNumber
            && InstalledKitchenEquipmentStateId == other.InstalledKitchenEquipmentStateId;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = (hash * 23) ^ SpaceNumber.GetHashCode();
            hash = (hash * 23) ^ (InstalledKitchenEquipmentStateId?.GetHashCode() ?? 0);
            return hash;
        }
    }
}
