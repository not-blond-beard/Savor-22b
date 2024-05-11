extends Control

const HouseSlotButtonScn = preload("res://scenes/village/house_slot_button.tscn")
const NoticePopupScn = preload("res://scenes/common/prefabs/notice_popup.tscn")
const ConfirmPopupScn = preload("res://scenes/common/prefabs/confirm_popup.tscn")
const GqlQuery = preload("res://gql/query.gd")

@onready var notice_popup = $MarginContainer/Background/Noticepopup
@onready var confirm_popup = $MarginContainer/Background/ConfirmPopup
@onready var grid_container = $MarginContainer/Background/MarginContainer/ScrollContainer/HomeGridContainer

var houses = []
var exist_houses = SceneContext.get_selected_village()["houses"]

func _ready():
	var size = SceneContext.selected_village_capacity
	
	grid_container.columns = SceneContext.selected_village_width
	
	var start_x_loc = -(( SceneContext.selected_village_width - 1 ) / 2)
	var start_y_loc = (SceneContext.selected_village_height -1 ) / 2
	var end_x_loc = ( SceneContext.selected_village_width - 1 ) / 2
	
	#create blank slots
	for i in range(size):
		var house = {"x" : start_x_loc, "y" : start_y_loc, "owner" : "none"}
		houses.append(house)
		
		if(start_x_loc == end_x_loc):
			start_y_loc -= 1
			start_x_loc = -(( SceneContext.selected_village_width - 1 ) / 2)
		else:
			start_x_loc += 1

	for h1 in exist_houses:
		for h2 in houses:
			if h1["x"] == h2["x"] and h1["y"] == h2["y"]:
				h2["owner"] = h1["owner"]
			
	for info in houses:
		var button = HouseSlotButtonScn.instantiate()
		button.set_house(info)
		button.button_down.connect(button_selected)
		grid_container.add_child(button)

func button_selected(house_index):
	var format_string1 = "house button down: %s"
	var format_string2 = "selected slot location: %s"
	SceneContext.selected_house_index = house_index
	SceneContext.selected_house_location = houses[house_index]

	#Toggle mode
	for slot in grid_container.get_children():
		if(slot.get_index() != house_index):
			slot.disable_button_selected()

func _on_button_pressed():
	get_tree().change_scene_to_file("res://scenes/village/select_village.tscn")

func _on_build_button_down():
	if (SceneContext.selected_house_location["owner"] != "none"):
		print_notice()
	else:
		var isHouseOwner = false
		var villageState = SceneContext.user_state["villageState"]
		if(villageState != null):
			isHouseOwner = true
		
		if (isHouseOwner):
			_query_relocation_cost_and_open()
		else:
			build_house()

func _query_relocation_cost_and_open():
	var gql_query = GqlQuery.new()
	gql_query.calculate_relocation_cost_query.graphql_response.connect(
		func(data):
			var confirm_popup_scn = ConfirmPopupScn.instantiate()
			
			confirm_popup_scn.set_label("%s 블록이 소요되며 %sBBG 가 필요합니다." % [
				str(data.data.calculateRelocationCost.durationBlocks),
				str(data.data.calculateRelocationCost.price)
			])
			confirm_popup_scn.ok_button_clicked_signal.connect(build_house)
			confirm_popup.add_child(confirm_popup_scn)
	)
	add_child(gql_query.calculate_relocation_cost_query)
	gql_query.calculate_relocation_cost_query.run({
		"villageId": SceneContext.user_state["villageState"]["houseState"]["villageId"],
		"relocationVillageId": SceneContext.get_selected_village()["id"]
	})
	
func print_notice():
	var box = NoticePopupScn.instantiate()
	notice_popup.add_child(box)
	
func build_house():
	var gql_query = GqlQuery.new()
	var query_string = gql_query.place_house_query_format.format([
			"\"%s\"" % GlobalSigner.signer.GetPublicKey(),
			SceneContext.get_selected_village()["id"],
			SceneContext.selected_house_location.x,
			SceneContext.selected_house_location.y], "{}")
		
	var query_executor = SvrGqlClient.raw(query_string)
	query_executor.graphql_response.connect(func(data):
		var unsigned_tx = data["data"]["createAction_PlaceUserHouse"]
		var signature = GlobalSigner.sign(unsigned_tx)
		var mutation_executor = SvrGqlClient.raw_mutation(
			gql_query.stage_tx_query_format % [unsigned_tx, signature]
		)
		add_child(mutation_executor)
		mutation_executor.run({})
	)
	add_child(query_executor)
	query_executor.run({})

func _on_refresh_button_down():
	Intro._query_villages()
	
	for child in grid_container.get_children():
		child.queue_free()
	
	for h0 in houses:
		h0["owner"] = "none"

	exist_houses = SceneContext.get_selected_village()["houses"]
	for h1 in exist_houses:
		for h2 in houses:
			if h1["x"] == h2["x"] and h1["y"] == h2["y"]:
				h2["owner"] = h1["owner"]
			
	for info in houses:
		var button = HouseSlotButtonScn.instantiate()
		button.set_house(info)
		button.button_down.connect(button_selected)
		grid_container.add_child(button)

func _on_back_button_down():
	get_tree().change_scene_to_file("res://scenes/village/village_view.tscn")
