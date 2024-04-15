namespace Savor22b.GraphTypes.Types;

using GraphQL;
using GraphQL.Types;
using Libplanet;
using Libplanet.Blockchain;
using Libplanet.Net;
using Libplanet.Store;
using Savor22b.Action.Exceptions;
using Savor22b.States;

public class UserDungeonStateType : ObjectGraphType<UserDungeonState>
{
    public UserDungeonStateType(
        BlockChain blockChain,
        BlockRenderer blockRenderer,
        IStore store,
        Swarm? swarm = null
    )
    {
        Field<NonNullGraphType<IntGraphType>>(
            name: "DungeonKeyCount",
            description: "The number of dungeon keys the user has.",
            resolve: context => context.Source.GetDungeonKeyCount(blockChain.Count)
        );

        Field<BooleanGraphType>(
            name: "IsDungeonConquestRewardReceivable",
            description: "점령한 던전의 주기적 보상을 받을 수 있는지 여부를 반환합니다. 점령한 던전이 아니라면 null을 반환합니다.",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "dungeonId",
                    Description = "보상을 받고자 하는 던전(점령한)의 ID",
                }
            ),
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
