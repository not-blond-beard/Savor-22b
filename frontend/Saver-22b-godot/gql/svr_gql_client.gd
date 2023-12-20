extends GQLClient

func _ready():
	print("client ready")
	set_endpoint(false, "192.168.2.9", 38080, "/graphql")
