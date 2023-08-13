namespace Savor22b.Tests.Fixtures;

using Savor22b.Helpers;
using Savor22b.Model;
using System.Collections.Immutable;

public class CsvDataFixture
{
    public ImmutableList<Recipe> Recipes { get; private set; }

    public CsvDataFixture() {
        CsvParser<Recipe> csvParser = new CsvParser<Recipe>();
        var csvPath = Paths.GetCSVDataPath("recipe.csv");
        Recipes = csvParser.ParseCsv(csvPath).ToImmutableList();
    }
}
