extends Node

var queryExecutor: GQLQueryExecuter

# Called when the node enters the scene tree for the first time.
func _ready():
	var query = GQLQuery.new("villages").set_props([
		"id",
		"name",
		"width",
		"height",
		"worldX",
		"worldY",
	])
	
	queryExecutor = SvrGqlClient.query('query', {}, query)
	queryExecutor.graphql_response.connect(self.graphql_response)
	add_child(queryExecutor)
	
	queryExecutor.run({})

func graphql_response(data: Dictionary):
	print(data)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
