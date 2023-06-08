namespace Savor22b.Tests.Fixtures;

using Savor22b.Helpers;
using Savor22b.Model;
using System.Collections.Immutable;

public class CsvDataFixture
{
    public ImmutableList<RecipeReference> RecipeReferences { get; private set; }

    public CsvDataFixture() {
        CsvParser<RecipeReference> csvParser = new CsvParser<RecipeReference>();
        var csvPath = Paths.GetCSVDataPath("recipe.csv");
        RecipeReferences = csvParser.ParseCsv(csvPath).ToImmutableList();
    }
}
