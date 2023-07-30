namespace Savor22b.States;


using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class HouseInnerState : State
{
    public Guid FirstBurnerEquipmentID { get; private set; }
    public Guid SecondBurnerEquipmentID { get; private set; }
    public Guid ThirdBurnerEquipmentID { get; private set; }


    public HouseInnerState()
    {
        FirstBurnerEquipmentID = Guid.Empty;
        SecondBurnerEquipmentID = Guid.Empty;
        ThirdBurnerEquipmentID = Guid.Empty;
    }

    public HouseInnerState(Guid? firstBurnerEquipmentID, Guid? secondBurnerEquipmentID, Guid? thirdBurnerEquipmentID)
    {
        FirstBurnerEquipmentID = firstBurnerEquipmentID ?? Guid.Empty;
        SecondBurnerEquipmentID = secondBurnerEquipmentID ?? Guid.Empty;
        ThirdBurnerEquipmentID = thirdBurnerEquipmentID ?? Guid.Empty;
    }

    public HouseInnerState(Bencodex.Types.Dictionary encoded)
    {
        if (encoded.TryGetValue((Text)nameof(FirstBurnerEquipmentID), out var firstBurnerEquipmentID))
        {
            FirstBurnerEquipmentID = firstBurnerEquipmentID.ToGuid();
        }
        else
        {
            FirstBurnerEquipmentID = Guid.Empty;
        }

        if (encoded.TryGetValue((Text)nameof(SecondBurnerEquipmentID), out var secondBurnerEquipmentID))
        {
            SecondBurnerEquipmentID = secondBurnerEquipmentID.ToGuid();
        }
        else
        {
            SecondBurnerEquipmentID = Guid.Empty;
        }

        if (encoded.TryGetValue((Text)nameof(ThirdBurnerEquipmentID), out var thirdBurnerEquipmentID))
        {
            ThirdBurnerEquipmentID = thirdBurnerEquipmentID.ToGuid();
        }
        else
        {
            ThirdBurnerEquipmentID = Guid.Empty;
        }
    }

    public IValue Serialize()
    {

        var pairs = new[]
        {
                new KeyValuePair<IKey, IValue>((Text)nameof(FirstBurnerEquipmentID), FirstBurnerEquipmentID.Serialize()),
                new KeyValuePair<IKey, IValue>((Text)nameof(SecondBurnerEquipmentID), SecondBurnerEquipmentID.Serialize()),
                new KeyValuePair<IKey, IValue>((Text)nameof(ThirdBurnerEquipmentID), ThirdBurnerEquipmentID.Serialize()),
        };
        return new Dictionary(pairs);
    }
}
