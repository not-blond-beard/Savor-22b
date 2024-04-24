namespace Savor22b.GraphTypes.Query;

using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Net;
using Savor22b.Action;

public class PeriodicDungeonRewardActionQuery : FieldType
{
    public PeriodicDungeonRewardActionQuery(BlockChain blockChain, Swarm swarm)
        : base()
    {
        Name = "createAction_PeriodicDungeonRewardAction";
        Type = typeof(NonNullGraphType<StringGraphType>);
        Description = "점령한 던전에 대한 주기적인 보상을 받습니다.";
        Arguments = new QueryArguments(
            new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = "publicKey",
                Description = "대상 유저의 40-hex 형태의 address 입니다.",
            },
            new QueryArgument<NonNullGraphType<IntGraphType>>
            {
                Name = "dungeonId",
                Description = "보상을 받는 대상 던전의 ID 입니다.",
            }
        );
        Resolver = new FuncFieldResolver<string>(context =>
        {
            try
            {
                var publicKey = new PublicKey(
                    Libplanet.ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new PeriodicDungeonRewardAction(context.GetArgument<int>("dungeonId"));

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
