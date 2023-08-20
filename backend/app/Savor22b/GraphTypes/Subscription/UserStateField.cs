namespace Savor22b.GraphTypes.Query;

using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Bencodex.Types;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Libplanet;
using Libplanet.Blockchain;
using Savor22b.GraphTypes.Types;
using Savor22b.States;

public class UserStateField : FieldType
{
    private readonly BlockChain _blockChain;
    private readonly Subject<Libplanet.Blocks.BlockHash> _subject;

    public UserStateField(BlockChain blockChain, Subject<Libplanet.Blocks.BlockHash> subject)
        : base()
    {
        _blockChain = blockChain;
        _subject = subject;

        Name = "UserState";
        Type = typeof(UserStateType);
        Description = "User State";
        Arguments = new QueryArguments(
            new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = "address",
                Description = "The account holder's 40-hex address",
            }
        );
        Resolver = new FuncFieldResolver<RootState>(context =>
        {
            var accountAddress = new Address(context.GetArgument<string>("address"));
            return GetRootState(accountAddress);
        });
        StreamResolver = new SourceStreamResolver<RootState>(
            (context) =>
            {
                var accountAddress = new Address(context.GetArgument<string>("address"));

                return _subject.DistinctUntilChanged().Select(_ => GetRootState(accountAddress));
            }
        );
    }

    private RootState GetRootState(Address address)
    {
        var rootStateEncoded = _blockChain.GetState(address);

        RootState rootState = rootStateEncoded is Dictionary bdict
            ? new RootState(bdict)
            : new RootState();

        return rootState;
    }
}
