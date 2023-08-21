using System.Collections.Immutable;

namespace Savor22b.Model;

public class VillageCharacteristic
{
    public int Id { get; set; }
    public int VillageId { get; set; }
    public ImmutableList<int> AvailableSeedIdList { get; set; }
}
