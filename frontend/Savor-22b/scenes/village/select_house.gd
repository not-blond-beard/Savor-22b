extends Control

const HouseSlotButtonScn = preload("res://scenes/village/house_slot_button.tscn")
const NoticePopupScn = preload("res://scenes/common/prefabs/notice_popup.tscn")
const ConfirmPopupScn = preload("res://scenes/common/prefabs/confirm_popup.tscn")

const HouseData = preload("res://gql/models/house.gd")
const DungeonData = preload("res://gql/models/dungeon.gd")
const VillageData = preload("res://gql/models/village.gd")

@onready var notice_popup = $MarginContainer/Background/Noticepopup
@onready var confirm_popup = $MarginContainer/Background/ConfirmPopup
@onready var grid_container = $MarginContainer/Background/MarginContainer/ScrollContainer/HomeGridContainer

var query_executor = QueryExecutor.new()
var place_house_query_executor
var stage_tx_mutation_executor
var get_houses_and_dungeons_query_executor
var calculate_relocation_cost_query_executor

var village_entities = []
var selected_entity = null

func register_query_executor():
	place_house_query_executor = query_executor.place_house_query_executor
	stage_tx_mutation_executor = query_executor.stage_tx_mutation_executor
	get_houses_and_dungeons_query_executor = query_executor.get_houses_and_dungeons_query_executor
	calculate_relocation_cost_query_executor = query_executor.calculate_relocation_cost_query_executor
	add_child(place_house_query_executor)
	add_child(stage_tx_mutation_executor)
	add_child(get_houses_and_dungeons_query_executor)
	add_child(calculate_relocation_cost_query_executor)

func _ready():
	register_query_executor()

	get_houses_and_dungeons_query_executor.graphql_response.connect(
		func(data):
			handle_houses_and_dungeons_response(data)
	)
	get_houses_and_dungeons_query_executor.run({})

func handle_houses_and_dungeons_response(data):
	var villages = []
	for village_data in data.get("data").get("villages"):
		var village = VillageData.new().from_dict(village_data)
		villages.append(village)

	draw_houses_and_dungeons(villages[SceneContext.selected_village_index])

func draw_houses_and_dungeons(village: VillageData):
	grid_container.columns = village.width

	for i in range(village.height):
		village_entities.append([])

	for y in range(1, village["height"] + 1):
		for x in range(1, village["width"] + 1):
			var entity = {"x": x, "y": y, "house": null, "dungeon": null}
			village_entities[y - 1].append(entity)

	for house in village.houses:
		village_entities[house.y - 1][house.x - 1].house = house
		
	for dungeon in village.dungeons:
		village_entities[dungeon.y - 1][dungeon.x - 1].dungeon = dungeon

	for row in village_entities:
		for entity in row:
			var button = HouseSlotButtonScn.instantiate()
			grid_container.add_child(button)
			button.set_entity(entity)
			button.button_down.connect(button_selected)

func button_selected(x: int, y: int):
	selected_entity = { "x": x, "y": y }

	# Calc lagacy house index
	var house_index = (y-1) * len(village_entities[0]) + x
	SceneContext.selected_house_index = house_index
	SceneContext.selected_house_location = village_entities[y - 1][x - 1]
#
	for slot in grid_container.get_children():
		if(slot.get_index() != house_index):
			slot.disable_button_selected()

func _on_button_pressed():
	get_tree().change_scene_to_file("res://scenes/village/select_village.tscn")

func _on_build_button_down():
	var dungeon = village_entities[selected_entity.y - 1][selected_entity.x - 1].get("dungeon", null)
	var house = village_entities[selected_entity.y - 1][selected_entity.x - 1].get("house", null)
	if (dungeon != null):
		print_notice()
	if (house != null):
		print_notice()
	else:
		var isHouseOwner = false
		var villageState = SceneContext.user_state["villageState"]
		if(villageState != null):
			isHouseOwner = true
		
		if (isHouseOwner):
			_query_relocation_cost_and_open()
		else:
			build_house(selected_entity.x, selected_entity.y)

func _query_relocation_cost_and_open():
	calculate_relocation_cost_query_executor.graphql_response.connect(
		func(data):
			var confirm_popup_scn = ConfirmPopupScn.instantiate()
			
			confirm_popup_scn.set_label("%s 블록이 소요되며 %sBBG 가 필요합니다." % [
				str(data.data.calculateRelocationCost.durationBlocks),
				str(data.data.calculateRelocationCost.price)
			])
			confirm_popup_scn.ok_button_clicked_signal.connect(build_house(selected_entity.x, selected_entity.y))
			confirm_popup.add_child(confirm_popup_scn)
	)

	calculate_relocation_cost_query_executor.run({
		"villageId": SceneContext.user_state["villageState"]["houseState"]["villageId"],
		"relocationVillageId": SceneContext.get_selected_village()["id"]
	})
	
func print_notice():
	var box = NoticePopupScn.instantiate()
	notice_popup.add_child(box)
	
func build_house(x: int, y: int):
	query_executor.stage_action(
		{
			"publicKey": GlobalSigner.signer.GetPublicKey(),
			"villageId": SceneContext.get_selected_village()["id"],
			"x": x,
			"y": y,
		},
		place_house_query_executor,
		stage_tx_mutation_executor
	)

func _on_refresh_button_down():
	Intro._query_villages()
	
	for child in grid_container.get_children():
		child.queue_free()
	
	village_entities = []

	get_houses_and_dungeons_query_executor.run({})

func _on_back_button_down():
	get_tree().change_scene_to_file("res://scenes/village/village_view.tscn")
