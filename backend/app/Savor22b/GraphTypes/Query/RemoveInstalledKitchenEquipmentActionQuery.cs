namespace Savor22b.GraphTypes.Query;

using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Net;
using Savor22b.Action;

public class RemoveInstalledKitchenEquipmentActionQuery : FieldType
{
    public RemoveInstalledKitchenEquipmentActionQuery(BlockChain blockChain, Swarm swarm)
        : base()
    {
        Name = "createAction_RemoveInstalledKitchenEquipmentActionQuery";
        Type = typeof(NonNullGraphType<StringGraphType>);
        Description = "설치된 큰 조리도구를 설치 제거합니다.";
        Arguments = new QueryArguments(
            new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = "publicKey",
                Description = "대상 유저의 40-hex 형태의 address 입니다.",
            },
            new QueryArgument<NonNullGraphType<GuidGraphType>>
            {
                Name = "installedEquipmentStateId",
                Description = "제거하려는 설치된 큰 조리도구의 State Id(Guid) 입니다.",
            }
        );
        Resolver = new FuncFieldResolver<string>(context =>
        {
            try
            {
                var publicKey = new PublicKey(
                    Libplanet.ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new RemoveInstalledKitchenEquipmentAction(
                    context.GetArgument<Guid>("installedEquipmentStateId")
                );

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
