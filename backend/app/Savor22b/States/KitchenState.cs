namespace Savor22b.States;


using Bencodex.Types;

public class KitchenState : State
{
    public KitchenState()
    {
        FirstApplianceSpace = new ApplianceSpaceState(1);
        SecondApplianceSpace = new ApplianceSpaceState(2);
        ThirdApplianceSpace = new ApplianceSpaceState(3);
    }

    public KitchenState(Dictionary encoded)
    {
        FirstApplianceSpace = new ApplianceSpaceState((Dictionary)encoded[nameof(FirstApplianceSpace)]);
        SecondApplianceSpace = new ApplianceSpaceState((Dictionary)encoded[nameof(SecondApplianceSpace)]);
        ThirdApplianceSpace = new ApplianceSpaceState((Dictionary)encoded[nameof(ThirdApplianceSpace)]);
    }

    public ApplianceSpaceState FirstApplianceSpace { get; private set; }

    public ApplianceSpaceState SecondApplianceSpace { get; private set; }

    public ApplianceSpaceState ThirdApplianceSpace { get; private set; }

    public IValue Serialize()
    {
        var pairs = new[]
        {
                new KeyValuePair<IKey, IValue>((Text)nameof(FirstApplianceSpace), FirstApplianceSpace.Serialize()),
                new KeyValuePair<IKey, IValue>((Text)nameof(SecondApplianceSpace), SecondApplianceSpace.Serialize()),
                new KeyValuePair<IKey, IValue>((Text)nameof(ThirdApplianceSpace), ThirdApplianceSpace.Serialize()),
        };
        return new Dictionary(pairs);
    }

    public void InstallKitchenEquipment(KitchenEquipmentState kitchenEquipmentState, int spaceNumber)
    {
        switch (spaceNumber)
        {
            case 1:
                FirstApplianceSpace = FirstApplianceSpace.InstallKitchenEquipment(kitchenEquipmentState.StateID);
                break;
            case 2:
                SecondApplianceSpace = SecondApplianceSpace.InstallKitchenEquipment(kitchenEquipmentState.StateID);
                break;
            case 3:
                ThirdApplianceSpace = ThirdApplianceSpace.InstallKitchenEquipment(kitchenEquipmentState.StateID);
                break;
            default:
                throw new ArgumentOutOfRangeException("KitchenState have only three appliance space");
        }
    }
}
