namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.States;

public class KitchenStateType : ObjectGraphType<KitchenState>
{
    public KitchenStateType()
    {
        Field<ApplianceSpaceStateType>(
            name: "FirstApplianceSpace",
            description: "The first appliance space.",
            resolve: context => context.Source.FirstApplianceSpace
        );
        Field<ApplianceSpaceStateType>(
            name: "SecondApplianceSpace",
            description: "The second appliance space.",
            resolve: context => context.Source.SecondApplianceSpace
        );
        Field<ApplianceSpaceStateType>(
            name: "ThirdApplianceSpace",
            description: "The third appliance space.",
            resolve: context => context.Source.ThirdApplianceSpace
        );
    }
}
