namespace Savor22b.GraphTypes.Query;

using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Libplanet;
using Libplanet.Blockchain;
using Libplanet.Net;
using Savor22b.Action;
using Libplanet.Tx;
using System.Collections.Immutable;
using Libplanet.Crypto;

public class ShowMeTheMoney : FieldType
{
    public ShowMeTheMoney(BlockChain blockChain, Swarm swarm)
        : base()
    {
        Name = "showMeTheMoney";
        Type = typeof(NonNullGraphType<StringGraphType>);
        Description = "BBG와 모든 아이템을 일정량 지급합니다.. ";
        Arguments = new QueryArguments(
            new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = "address",
                Description = "재화를 받을 주소. ",
            }
        );
        Resolver = new FuncFieldResolver<string>(context =>
        {
            try
            {
                PrivateKey privateKey = PrivateKey.FromString("eda6ef63ae945cd15572fcf7d6635a8b3f8d86e85b57a353b482bc82c7fd2ad4");

                var action = new ShowMeTheMoneyAction(new Address(context.GetArgument<string>("address")));

                var unsignedTxHex = new GetUnsignedTransactionHex(
                    action,
                    privateKey.PublicKey,
                    blockChain,
                    swarm
                ).UnsignedTransactionHex;

                byte[] signature = privateKey.Sign(ByteUtil.ParseHex(unsignedTxHex));

                IUnsignedTx unsignedTransaction = TxMarshaler.DeserializeUnsignedTx(
                    ByteUtil.ParseHex(unsignedTxHex)
                );

                Transaction signedTransaction = new Transaction(
                    unsignedTransaction,
                    signature.ToImmutableArray()
                );

                blockChain.StageTransaction(signedTransaction);
                swarm?.BroadcastTxs(new[] { signedTransaction });

                return "success";
            }
            catch (Exception e)
            {
                throw new ExecutionError(e.Message);
            }
        });
    }
}
