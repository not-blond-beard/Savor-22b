namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Libplanet;
using Libplanet.Blockchain;
using Libplanet.Explorer.GraphTypes;
using Savor22b.Model;

public class DungeonStateType : ObjectGraphType<Dungeon>
{
    public DungeonStateType(BlockChain blockChain)
    {
        Field<NonNullGraphType<StringGraphType>>(
            "name",
            description: "던전의 이름입니다.",
            resolve: context => context.Source.Name
        );

        Field<NonNullGraphType<IntGraphType>>(
            "x",
            description: "던전이 위치하는 마을의 x 좌표입니다.",
            resolve: context => context.Source.X
        );

        Field<NonNullGraphType<IntGraphType>>(
            "y",
            description: "던전이 위치하는 마을의 y 좌표입니다.",
            resolve: context => context.Source.Y
        );

        Field<NonNullGraphType<IntGraphType>>(
            "id",
            description: "던전의 유니크 아이디입니다.",
            resolve: context => context.Source.ID
        );

        Field<NonNullGraphType<IntGraphType>>(
            "villageId",
            description: "던전이 위치하는 마을의 id 입니다.",
            resolve: context => context.Source.VillageId
        );

        Field<NonNullGraphType<ListGraphType<StaticSeedStateType>>>(
            "seeds",
            description: "던전에서 획득 가능한 씨앗 목록입니다.",
            resolve: context =>
            {
                var seeds = context.Source.RewardSeedIdList.Select(
                    seedId => CsvDataHelper.GetSeedById(seedId)!
                );

                return seeds;
            }
        );

        Field<NonNullGraphType<BooleanGraphType>>(
            "isConquest",
            description: "현재 던전이 점령 되었는지의 여부입니다.",
            resolve: context => context.Source.IsConquest(blockChain)
        );

        Field<AddressType>(
            "conquestUserAddress",
            description: "현재 던전을 점령하고 있는 유저의 Address입니다.",
            resolve: context => context.Source.CurrentConquestUserAddress(blockChain)
        );
    }
}
