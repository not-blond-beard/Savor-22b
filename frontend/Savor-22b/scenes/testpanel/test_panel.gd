extends Control

const Gql_query = preload("res://gql/query.gd")

@onready var label = $Background/MarginContainer/GridContainer/Label
@onready var lineedit = $Background/MarginContainer/GridContainer/LineEdit
func _ready():
	pass

func _input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_T:
			get_tree().change_scene_to_file("res://scenes/testpanel/test_panel.tscn")
			print("Test panel open")


func _on_button_pressed():
	get_tree().change_scene_to_file("res://scenes/farm.tscn")


func _on_button_2_pressed():
	
	var test = SceneContext.shop
	print(test)


func _on_button_3_pressed():
	test4()

# use \" on every double quotation

func test4():
	var string = lineedit.text
	print(string)
	var gql_query = Gql_query.new()
	var query_string = "query
{
  createAction_CreateFood(publicKey:\"044B83CB8CE52392AD9E46FAF398F96C5CD7CDB95A9EA990A9A55CC575237D2B342D3C43AB5E6E149B87F82544769D70E93A10B0B38D9B579E0A895BF58CB7780F\",
  recipeID: 2,
  refrigeratorStateIdsToUse: [\"7435f34b-39f6-4829-a6df-4e6f9affab06\",\"aa2c9ec2-2788-4afa-8ee1-d43ec50127a3\",\"bda2e49a-ec87-4e42-bbab-e73488e8d750\"],
  kitchenEquipmentStateIdsToUse: \"27d27975-b141-4963-b72a-d4cb0e71bc63\")
}"

	print(query_string)
	
	var query_executor = SvrGqlClient.raw(query_string)
	query_executor.graphql_response.connect(func(data):
		print("gql response: ", data)
		var unsigned_tx = data["data"]["createAction_CreateFood"]
		print("unsigned tx: ", unsigned_tx)
		var signature = GlobalSigner.sign(unsigned_tx)
		print("signed tx: ", signature)
		var mutation_executor = SvrGqlClient.raw_mutation(gql_query.stage_tx_query_format % [unsigned_tx, signature])
		mutation_executor.graphql_response.connect(func(data):
			print("mutation res: ", data)
		)
		add_child(mutation_executor)
		mutation_executor.run({})
	)
	add_child(query_executor)
	query_executor.run({})


func _on_button_4_pressed():
	test4()
