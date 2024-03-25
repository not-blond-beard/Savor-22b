using GraphQL.Types;
using Savor22b.Model;

namespace Savor22b.GraphTypes.Types;

public class StaticSeedStateType : ObjectGraphType<Seed>
{
    public StaticSeedStateType()
    {
        Field<IntGraphType>(
            name: "seedId",
            description: "The ID of the seed.",
            resolve: context => context.Source.Id
        );

        Field<StringGraphType>(
            name: "name",
            description: "The name of the seed.",
            resolve: context =>
            {
                Seed seed = CsvDataHelper.GetSeedById(context.Source.Id)!;
                return seed.Name;
            }
        );
    }
}
