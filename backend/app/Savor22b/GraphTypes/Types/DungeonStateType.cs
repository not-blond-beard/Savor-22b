namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.Model;

public class DungeonStateType : ObjectGraphType<Dungeon>
{
    public DungeonStateType()
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
    }
}
