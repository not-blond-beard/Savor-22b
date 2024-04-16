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
        Field<NonNullGraphType<IntGraphType>>(
            name: "DungeonConquestKeyCount",
            description: "해당 던전에 점령 신청 할 수 있는 키의 남은 갯수입니다.",
            resolve: context =>
            {
                int dungeonId = context.GetArgument<int>("dungeonId");
                return context.Source.GetDungeonConquestKeyCount(dungeonId, blockChain.Count);
            }
        );

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

        Field<NonNullGraphType<ListGraphType<DungeonHistoryStateType>>>(
            name: "DungeonHistories",
            description: "던전의 탐험 기록(클리어 기록)을 반환합니다.",
            resolve: context => context.Source.DungeonHistories
        );

        Field<NonNullGraphType<ListGraphType<DungeonConquestHistoryStateType>>>(
            name: "DungeonConquestHistories",
            description: "던전의 점령 기록을 반환합니다.",
            resolve: context => context.Source.DungeonConquestHistories
        );
    }
}
