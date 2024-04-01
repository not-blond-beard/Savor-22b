namespace Savor22b.GraphTypes.Query;

using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Libplanet.Blockchain;
using Libplanet.Net;
using Savor22b.DataModel;
using Savor22b.GraphTypes.Types;
using Savor22b.Model;

public class DungeonReturnRewardQuery : FieldType
{
    public DungeonReturnRewardQuery()
        : base()
    {
        Name = "dungeonReturnReward";
        Type = typeof(NonNullGraphType<DungeonReturnRewardType>);
        Description = "특정 던전 점령권을 반환했을 때 얻을 수 있는 보상을 반환합니다. ";
        Arguments = new QueryArguments(
            new QueryArgument<NonNullGraphType<IntGraphType>>
            {
                Name = "dungeonId",
                Description = "점령권을 반환할 예정인 던전 Id를 입력합니다. ",
            }
        );
        Resolver = new FuncFieldResolver<DungeonReturnReward>(context =>
        {
            try
            {
                var dungeonId = context.GetArgument<int>("dungeonId");
                Dungeon? dungeon = CsvDataHelper.GetDungeonById(dungeonId);

                if (dungeon == null)
                {
                    throw new ExecutionError("해당 던전이 존재하지 않습니다. ");
                }

                return new DungeonReturnReward(dungeon.ReturnDungeonRewardBBG);
            }
            catch (Exception e)
            {
                throw new ExecutionError(e.Message);
            }
        });
    }
}
