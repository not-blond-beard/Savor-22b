namespace Savor22b.GraphTypes.Types;

using System.Collections.Immutable;
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

        Field<NonNullGraphType<IntGraphType>>(
            name: "MaxDungeonKeyCount",
            description: "던전 키의 최대 개수입니다.",
            resolve: context => UserDungeonState.MaxDungeonConquestKeyCount
        );

        Field<NonNullGraphType<LongGraphType>>(
            name: "DungeonKeyChargeIntervalBlock",
            description: "던전 키의 충전 주기(Block)입니다.",
            resolve: context => UserDungeonState.DungeonKeyChargeIntervalBlock
        );

        Field<NonNullGraphType<UserDungeonDetailType>>(
            name: "DungeonDetail",
            description: "특정 던전에 대한(유저의) 상세 정보입니다.",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "dungeonId",
                    Description = "조회할 던전의 ID입니다.",
                }
            ),
            resolve: context =>
            {
                return context.Source;
            }
        );
    }
}
