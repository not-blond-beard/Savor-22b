using Libplanet;

namespace Savor22b.GraphTypes.Types;

public class House
{
    public int VillageId { get; private set; }
    public int X { get; private set; }

    public int Y { get; private set; }

    public Address Owner { get; private set; }

    public House(int villageId, int x, int y, Address owner)
    {
        VillageId = villageId;
        X = x;
        Y = y;
        Owner = owner;
    }
};
