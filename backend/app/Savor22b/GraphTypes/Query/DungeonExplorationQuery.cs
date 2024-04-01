namespace Savor22b.GraphTypes.Query;

using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Net;
using Savor22b.Action;

public class DungeonExplorationQuery : FieldType
{
    public DungeonExplorationQuery(BlockChain blockChain, Swarm swarm)
        : base()
    {
        Name = "createAction_DungeonExploration";
        Type = typeof(NonNullGraphType<StringGraphType>);
        Description = "던전 키를 소모하여 특정 던전의 스테이지를 모두 돌고, 클리어를 시도합니다.";
        Arguments = new QueryArguments(
            new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = "publicKey",
                Description = "대상 유저의 40-hex 형태의 address 입니다.",
            },
            new QueryArgument<NonNullGraphType<IntGraphType>>
            {
                Name = "dungeonId",
                Description = "탐험할 던전의 ID 입니다.",
            }
        );
        Resolver = new FuncFieldResolver<string>(context =>
        {
            try
            {
                var publicKey = new PublicKey(
                    Libplanet.ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new ExplorationDungeonAction(context.GetArgument<int>("dungeonId"));

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    blockChain,
                    swarm
                ).UnsignedTransactionHex;
            }
            catch (Exception e)
            {
                throw new ExecutionError(e.Message);
            }
        });
    }
}
