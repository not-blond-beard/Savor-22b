using Godot;
using GraphQL.Client;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using System.Threading.Tasks;

public partial class GraphQLClient : Node
{
	private GraphQLHttpClient client;

	public static GraphQLClient Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;

		client = new GraphQLHttpClient("http://localhost:38080/graphql", new NewtonsoftJsonSerializer());
	}

	public async Task<string> QueryAsync(string query)
	{
		var request = new GraphQLHttpRequest { Query = query };
		var response = await client.SendQueryAsync<dynamic>(request);
		if (response.Errors != null)
		{
			GD.Print("Error: ", response.Errors[0].Message);
		}
		// else
		// {
		// 	GD.Print("Response: ", response.Data);
		// }
		return response.Data;
	}
}
