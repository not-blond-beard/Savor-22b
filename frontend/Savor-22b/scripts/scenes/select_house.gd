extends Control

const SELECT_HOUSE_BUTTON = preload("res://ui/house_slot_button.tscn")
const SLOT_IS_FULL = preload("res://ui/notice_popup.tscn")
const Gql_query = preload("res://gql/query.gd")

@onready var noticepopup = $MarginContainer/Background/Noticepopup
@onready var gridcontainer = $MarginContainer/Background/MarginContainer/ScrollContainer/HomeGridContainer

var houses = []
var existhouses = SceneContext.get_selected_village()["houses"]


func _ready():
	print("select_house scene ready")
	var size = SceneContext.selected_village_capacity
	
	gridcontainer.columns = SceneContext.selected_village_width
	
	var startxloc = -(( SceneContext.selected_village_width - 1 ) / 2)
	var startyloc = (SceneContext.selected_village_height -1 ) / 2
	var endxloc = ( SceneContext.selected_village_width - 1 ) / 2
	
	print("startxloc: %s" % startxloc)
	print("startyloc: %s" % startyloc)
	print("endxloc: %s" % endxloc)

	
	#create blank slots
	for i in range(size):
		var house = {"x" : startxloc, "y" : startyloc, "owner" : "none"}
		houses.append(house)
		
		if(startxloc == endxloc):
			startyloc -= 1
			startxloc = -(( SceneContext.selected_village_width - 1 ) / 2)
		else:
			startxloc += 1

	for h1 in existhouses:
		for h2 in houses:
			if h1["x"] == h2["x"] and h1["y"] == h2["y"]:
				h2["owner"] = h1["owner"]
			
	for info in houses:
		var button = SELECT_HOUSE_BUTTON.instantiate()
		button.set_house(info)
		button.button_down.connect(button_selected)
		gridcontainer.add_child(button)


func button_selected(house_index):
	var format_string1 = "house button down: %s"
	var format_string2 = "selected slot location: %s"
	print(format_string1 % house_index)
	print(format_string2 % houses[house_index])	
	SceneContext.selected_house_index = house_index
	SceneContext.selected_house_location = houses[house_index]


func _on_button_pressed():
	get_tree().change_scene_to_file("res://scenes/select_village.tscn")


func _on_build_button_button_down():
	if (SceneContext.selected_house_location["owner"] != "none"):
		print_notice()
	else:
		build_house()


	
func print_notice():
	var box = SLOT_IS_FULL.instantiate()
	noticepopup.add_child(box)
	
func build_house():
	var gql_query = Gql_query.new()
	var query_string = gql_query.place_house_query_format.format([
			"\"%s\"" % GlobalSigner.signer.GetPublicKey(),
			SceneContext.get_selected_village()["id"],
			SceneContext.selected_house_location.x,
			SceneContext.selected_house_location.y], "{}")
	print(query_string)
	
	var query_executor = SvrGqlClient.raw(query_string)
	query_executor.graphql_response.connect(func(data):
		print("gql response: ", data)
		var unsigned_tx = data["data"]["createAction_PlaceUserHouse"]
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



func _on_refresh_button_button_down():
	Intro._query_villages()
	
	for child in gridcontainer.get_children():
		child.queue_free()
	
	for h0 in houses:
		h0["owner"] = "none"

	existhouses = SceneContext.get_selected_village()["houses"]
	for h1 in existhouses:
		for h2 in houses:
			if h1["x"] == h2["x"] and h1["y"] == h2["y"]:
				h2["owner"] = h1["owner"]
			
	for info in houses:
		var button = SELECT_HOUSE_BUTTON.instantiate()
		button.set_house(info)
		button.button_down.connect(button_selected)
		gridcontainer.add_child(button)
