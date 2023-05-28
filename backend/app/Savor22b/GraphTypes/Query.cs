namespace Savor22b.GraphTypes;

using System.Diagnostics.CodeAnalysis;
using GraphQL;
using GraphQL.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Blockchain;
using Libplanet.Net;
using Savor22b.Action;
using Savor22b.States;

public class Query : ObjectGraphType
{
    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "GraphQL docs require long lines of text.")]
    public Query(
        BlockChain blockChain,
        Swarm? swarm = null)
    {
        Field<StringGraphType>(
            "asset",
            description: "The specified address's balance in MNT.",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "address",
                    Description = "The account holder's 40-hex address",
                }
            ),
            resolve: context =>
            {
                var accountAddress = new Address(context.GetArgument<string>("address"));
                FungibleAssetValue asset = blockChain.GetBalance(
                    accountAddress,
                    Currencies.KeyCurrency
                );

                return asset.ToString();
            }
        );

        // TODO: Move to Libplanet.Explorer or Node API.
        Field<StringGraphType>(
            "peerString",
            resolve: context =>
                swarm is null
                ? throw new InvalidOperationException("Network settings is not set.")
                : swarm.AsPeer.PeerString);

        Field<InventoryStateType>(
           "inventory",
           description: "The inventory of the specified address.",
           arguments: new QueryArguments(
               new QueryArgument<NonNullGraphType<StringGraphType>>
               {
                   Name = "address",
                   Description = "The account holder's 40-hex address",
               }
           ),
           resolve: context =>
           {
               var accountAddress = new Address(context.GetArgument<string>("address"));

               // Retrieve InventoryState from the blockchain based on the account address.
               var inventoryStateEncoded = blockChain.GetState(accountAddress);

               InventoryState inventoryState =
                    inventoryStateEncoded is Bencodex.Types.Dictionary bdict
                        ? new InventoryState(bdict)
                        : new InventoryState();

               return inventoryState;
           }
       );

    }
}
