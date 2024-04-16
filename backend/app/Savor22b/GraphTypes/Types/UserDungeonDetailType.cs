namespace Savor22b.GraphTypes.Types;

using GraphQL;
using GraphQL.Types;
using Libplanet.Blockchain;
using Libplanet.Net;
using Libplanet.Store;
using Savor22b.States;

public class UserDungeonDetailType : ObjectGraphType<UserDungeonState>
{
    public UserDungeonDetailType(
        BlockChain blockChain,
        BlockRenderer blockRenderer,
        IStore store,
        Swarm? swarm = null
    )
    {
        Field<BooleanGraphType>(
            name: "IsDungeonConquestRewardReceivable",
            description: "점령한 던전의 주기적 보상을 받을 수 있는지 여부를 반환합니다. 점령한 던전이 아니라면 null을 반환합니다.",
            resolve: context =>
            {
                int dungeonId = context.GetArgument<int>("dungeonId");

                DungeonConquestHistoryState? history = context.Source.CurrentConquestDungeonHistory(
                    dungeonId
                );

                if (history is null)
                {
                    return null;
                }

                return context.Source.IsDungeonConquestRewardReceivable(
                    dungeonId,
                    history.BlockIndex,
                    blockChain.Count
                );
            }
        );
    }
}
