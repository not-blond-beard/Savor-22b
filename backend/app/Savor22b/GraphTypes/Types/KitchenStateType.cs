namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.States;

public class KitchenStateType : ObjectGraphType<KitchenState>
{
    public KitchenStateType()
    {
        Field<GuidGraphType>(
            name: "FirstApplianceSpace",
            description: "The ID of the first burner equipment.",
            resolve: context => context.Source.FirstApplianceSpace
        );
        Field<GuidGraphType>(
            name: "SecondApplianceSpace",
            description: "The ID of the second burner equipment.",
            resolve: context => context.Source.SecondApplianceSpace
        );
        Field<GuidGraphType>(
            name: "ThirdApplianceSpace",
            description: "The ID of the third burner equipment.",
            resolve: context => context.Source.ThirdApplianceSpace
        );
    }
}
