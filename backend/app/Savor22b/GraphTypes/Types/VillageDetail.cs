using System.Collections.Immutable;
using Savor22b.Model;

namespace Savor22b.GraphTypes.Types;

public class VillageDetail : Village
{
    public ImmutableList<House> Houses { get; set; }

    public VillageDetail(Village village, ImmutableList<House> houses)
        : base()
    {
        Id = village.Id;
        Name = village.Name;
        Width = village.Width;
        Height = village.Height;
        WorldX = village.WorldX;
        WorldY = village.WorldY;

        Houses = houses;
    }

    public VillageDetail(Village village)
        : base()
    {
        Id = village.Id;
        Name = village.Name;
        Width = village.Width;
        Height = village.Height;
        WorldX = village.WorldX;
        WorldY = village.WorldY;

        Houses = ImmutableList<House>.Empty;
    }

    public VillageDetail UpdateHouses(ImmutableList<House> houses)
    {
        return new VillageDetail(this, houses);
    }
}
