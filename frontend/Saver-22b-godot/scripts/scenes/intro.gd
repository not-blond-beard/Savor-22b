extends Control

func _ready():
	print("intro scene ready")
	_query_villages()

func _on_quit_button_button_down():
	print("quit button down")
	get_tree().quit()

func _on_play_button_button_down():
	print("play button down")
	get_tree().change_scene_to_file("res://scenes/select_village.tscn")

func _query_villages():
	var query = GQLQuery.new("villages").set_props([
		"id",
		"name",
		"width",
		"height",
		"worldX",
		"worldY",
		GQLQuery.new("houses").set_props([
			"x",
			"y",
			"owner",
		]),
	])
	
	var query_executor = SvrGqlClient.query('query', {}, query)
	query_executor.graphql_response.connect(func(data):
		SceneContext.set_villages(data))
	add_child(query_executor)
	query_executor.run({})
