namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Libplanet.Explorer.GraphTypes;
using Savor22b.DataModel;

public class DungeonReturnRewardType : ObjectGraphType<DungeonReturnReward>
{
    public DungeonReturnRewardType()
    {
        Field<NonNullGraphType<StringGraphType>>(
            "rewardBBG",
            description: "던전 점령권 반환 시 획득 가능한 BBG입니다. ",
            resolve: context => context.Source.RewardBBG
        );
    }
}
