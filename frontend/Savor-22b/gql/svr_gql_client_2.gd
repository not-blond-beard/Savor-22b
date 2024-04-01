extends GQLClient2

func _ready():
	print("client 2 ready")
	#set_endpoint(false, "1.230.253.103", 38080, "/graphql")
	
	set_endpoint_2(false, "localhost", 38080, "/graphql/explorer")
	

func raw_mutation(query:String):
	return GQLQueryExecuter2.new(endpoint, use_ssl, Mutation.new(query))
