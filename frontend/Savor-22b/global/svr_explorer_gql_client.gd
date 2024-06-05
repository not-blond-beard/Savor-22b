extends GQLClient

func _ready():
		
	set_endpoint(false, "localhost", 38080, "/graphql/explorer")
	

func raw_mutation(query:String):
	return GQLQueryExecuter.new(endpoint, use_ssl, Mutation.new(query))
