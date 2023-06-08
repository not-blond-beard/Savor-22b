namespace Savor22b.GraphTypes;

using GraphQL.Types;

public class RecipeGraphType
{

    public class RecipeType : ObjectGraphType<Recipe>
    {
        public RecipeType()
        {
            Field<IntGraphType>(
                name: "id",
                description: "The ID of the recipe.",
                resolve: context =>
                {
                    return context.Source.Id;
                }
            );

            Field<StringGraphType>(
                name: "name",
                description: "The name of the recipe.",
                resolve: context => context.Source.Name
            );

            Field<ListGraphType<IngredientType>>(
                name: "ingredients",
                description: "The list of ingredients in the recipe.",
                resolve: context => context.Source.Ingredients.ToList()
            );

            Field<StringGraphType>(
                name: "minGrade",
                description: "The minimum grade of the recipe.",
                resolve: context => context.Source.MinGrade

            );

            Field<StringGraphType>(
                name: "maxGrade",
                description: "The maximum grade of the recipe.",
                resolve: context => context.Source.MaxGrade
            );
        }
    }

    public class IngredientType : ObjectGraphType<RecipeIngredient>
    {
        public IngredientType()
        {
            Field<IntGraphType>(
                name: "id",
                description: "The ID of the ingredient.",
                resolve: context =>
                {
                    return context.Source.Id;
                }
            );

            Field<StringGraphType>(
                name: "name",
                description: "The name of the ingredient.",
                resolve: context => context.Source.Name
            );

            Field<StringGraphType>(
                name: "type",
                description: "The type of the ingredient.",
                resolve: context => context.Source.Type
            );
        }
    }

}
