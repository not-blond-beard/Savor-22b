namespace Savor22b.States;


using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class KitchenState : State
{
    public Guid FirstApplianceSpace { get; private set; }

    public Guid SecondApplianceSpace { get; private set; }

    public Guid ThirdApplianceSpace { get; private set; }

    public KitchenState()
    {
        FirstApplianceSpace = Guid.Empty;
        SecondApplianceSpace = Guid.Empty;
        ThirdApplianceSpace = Guid.Empty;
    }

    public KitchenState(Guid? firstApplianceSpace, Guid? secondApplianceSpace, Guid? thirdApplianceSpace)
    {
        FirstApplianceSpace = firstApplianceSpace ?? Guid.Empty;
        SecondApplianceSpace = secondApplianceSpace ?? Guid.Empty;
        ThirdApplianceSpace = thirdApplianceSpace ?? Guid.Empty;
    }

    public KitchenState(Bencodex.Types.Dictionary encoded)
    {
        if (encoded.TryGetValue((Text)nameof(FirstApplianceSpace), out var firstApplianceSpace))
        {
            FirstApplianceSpace = firstApplianceSpace.ToGuid();
        }
        else
        {
            FirstApplianceSpace = Guid.Empty;
        }

        if (encoded.TryGetValue((Text)nameof(SecondApplianceSpace), out var secondApplianceSpace))
        {
            SecondApplianceSpace = secondApplianceSpace.ToGuid();
        }
        else
        {
            SecondApplianceSpace = Guid.Empty;
        }

        if (encoded.TryGetValue((Text)nameof(ThirdApplianceSpace), out var thirdApplianceSpace))
        {
            ThirdApplianceSpace = thirdApplianceSpace.ToGuid();
        }
        else
        {
            ThirdApplianceSpace = Guid.Empty;
        }
    }

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
                FirstApplianceSpace = kitchenEquipmentState.StateID;
                break;
            case 2:
                SecondApplianceSpace = kitchenEquipmentState.StateID;
                break;
            case 3:
                ThirdApplianceSpace = kitchenEquipmentState.StateID;
                break;
            default:
                throw new ArgumentOutOfRangeException("KitchenState have only three appliance space");
        }
    }
}
