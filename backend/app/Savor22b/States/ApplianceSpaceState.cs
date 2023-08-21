namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class ApplianceSpaceState : State
{
    public ApplianceSpaceState(int spaceNumber)
    {
        SpaceNumber = spaceNumber;
        InstalledKitchenEquipmentStateId = null;
    }

    public ApplianceSpaceState(int spaceNumber, Guid? installedKitchenEquipmentStateId)
    {
        SpaceNumber = spaceNumber;
        InstalledKitchenEquipmentStateId = installedKitchenEquipmentStateId;
    }

    public ApplianceSpaceState(Dictionary encoded)
    {
        SpaceNumber = encoded[nameof(SpaceNumber)].ToInteger();
        InstalledKitchenEquipmentStateId = encoded[
            nameof(InstalledKitchenEquipmentStateId)
        ].ToNullableGuid();
    }

    public int SpaceNumber { get; private set; }

    public Guid? InstalledKitchenEquipmentStateId { get; private set; }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(SpaceNumber), SpaceNumber.Serialize()),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(InstalledKitchenEquipmentStateId),
                InstalledKitchenEquipmentStateId.Serialize()
            ),
        };
        return new Dictionary(pairs);
    }

    public ApplianceSpaceState InstallKitchenEquipment(Guid installedKitchenEquipmentStateId)
    {
        return new ApplianceSpaceState(SpaceNumber, installedKitchenEquipmentStateId);
    }

    public void UnInstallKitchenEquipment()
    {
        InstalledKitchenEquipmentStateId = null;
    }

    public bool EquipmentIsPresent()
    {
        return InstalledKitchenEquipmentStateId is not null;
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
