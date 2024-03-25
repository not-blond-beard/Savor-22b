using System.Collections.Immutable;

namespace Savor22b.Model;

public class Dungeon
{
    public string Name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int ID { get; set; }
    public int VillageId { get; set; }
    public ImmutableList<int> RewardSeedIdList { get; set; }
}
