extends GQLClient

func _ready():
	print("client ready")
	set_endpoint(false, "localhost", 38080, "/graphql")
