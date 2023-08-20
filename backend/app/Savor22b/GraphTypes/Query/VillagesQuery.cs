namespace Savor22b.GraphTypes.Query;

using System.Collections.Immutable;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Savor22b.GraphTypes.Types;
using Savor22b.Model;

public class VillagesQuery : FieldType
{
    public VillagesQuery()
        : base()
    {
        Name = "villages";
        Type = typeof(NonNullGraphType<ListGraphType<VillageType>>);
        Description = "Get all villages";
        Resolver = new FuncFieldResolver<ImmutableList<Village>>(context =>
        {
            try
            {
                ImmutableList<Village> villages = CsvDataHelper.GetVillageCSVData();
                return villages;
            }
            catch (Exception e)
            {
                throw new ExecutionError(e.Message);
            }
        });
    }
}
