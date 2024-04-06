namespace Savor22b.GraphTypes.Subscription;

using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Bencodex.Types;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Libplanet.Blockchain;
using Savor22b.Constants;
using Savor22b.GraphTypes.Types;
using Savor22b.Model;
using Savor22b.States;

public class VillageField : FieldType
{
    private readonly BlockChain _blockChain;
    private readonly Subject<Libplanet.Blocks.BlockHash>? _subject;

    public VillageField(BlockChain blockChain, Subject<Libplanet.Blocks.BlockHash>? subject = null)
        : base()
    {
        _blockChain = blockChain;
        _subject = subject;

        Name = "Village";
        Type = typeof(NonNullGraphType<ListGraphType<VillageType>>);
        Description = "Get all villages";
        Resolver = new FuncFieldResolver<ImmutableList<VillageDetail>>(context =>
        {
            try
            {
                var villages = CsvDataHelper.GetVillageCSVData().ToArray();
                var villageDetails = GetVillageDetails(
                    villages.ToImmutableList(),
                    context,
                    blockChain
                );

                return villageDetails;
            }
            catch (Exception e)
            {
                throw new ExecutionError(e.Message);
            }
        });
        StreamResolver = new SourceStreamResolver<ImmutableList<VillageDetail>>(
            (context) =>
            {
                try
                {
                    var villages = CsvDataHelper.GetVillageCSVData().ToArray();

                    return _subject!
                        .DistinctUntilChanged()
                        .Select(
                            _ => GetVillageDetails(villages.ToImmutableList(), context, blockChain)
                        );
                }
                catch (Exception e)
                {
                    throw new ExecutionError(e.Message);
                }
            }
        );
    }

    public static ImmutableList<VillageDetail> GetVillageDetails(
        ImmutableList<Village> villages,
        IResolveFieldContext context,
        BlockChain blockChain
    )
    {
        GlobalUserHouseState globalUserHouseState = blockChain.GetState(
            Addresses.UserHouseDataAddress
        )
            is Dictionary stateEncoded
            ? new GlobalUserHouseState(stateEncoded)
            : new GlobalUserHouseState();

        var houseStates = globalUserHouseState.UserHouse.ToImmutableList();
        var houses = new Dictionary<int, List<House>>();

        foreach (var houseState in houseStates)
        {
            House house = GlobalUserHouseState.ParsingKey(houseState.Key, houseState.Value);

            if (houses.ContainsKey(house.VillageId))
            {
                houses[house.VillageId].Add(house);
            }
            else
            {
                houses[house.VillageId] = new List<House> { house };
            }
        }

        var villageDetails = villages
            .Select(village =>
            {
                var housesInVillage = houses.ContainsKey(village.Id)
                    ? houses[village.Id].ToImmutableList()
                    : ImmutableList<House>.Empty;

                return new VillageDetail(village, housesInVillage);
            })
            .ToImmutableList();

        return villageDetails;
    }
}
