extends Control

const DoneNoticePopupScn = preload("res://scenes/common/prefabs/done_notice_popup.tscn")

const FarmSlotEmptyScn = preload("res://scenes/farm/farm_slot_empty.tscn")
const FarmSlotButtonScn = preload("res://scenes/farm/farm_slot_button.tscn")
const FarmSlotDoneScn = preload("res://scenes/farm/farm_slot_done.tscn")
const FarmInstallPopupScn = preload("res://scenes/farm/farm_install_popup.tscn")
const FarmActionPopupScn = preload("res://scenes/farm/farm_action_popup.tscn")
const FarmAskRemovePopupScn = preload("res://scenes/farm/farm_ask_remove_popup.tscn")
const FarmRemoveDonePopupScn = preload("res://scenes/farm/farm_remove_done_popup.tscn")


@onready var left_farm = $MC/HC/CR/MC/HC/Left
@onready var right_farm = $MC/HC/CR/MC/HC/Right
@onready var popup_area = $Popups

var farms = []
var item_state_ids = []
var item_state_Id_to_use
var harvested_name
var action_success = false

var query_executor = QueryExecutor.new()
var remove_seed_query_executor
var remove_weed_query_executor
var plant_seed_query_executor
var harvest_seed_query_executor
var stage_tx_mutation_executor

func _ready():
	remove_seed_query_executor = query_executor.remove_seed_query_executor
	remove_weed_query_executor = query_executor.remove_weed_query_executor
	plant_seed_query_executor = query_executor.plant_seed_query_executor
	harvest_seed_query_executor = query_executor.harvest_seed_query_executor
	stage_tx_mutation_executor = query_executor.stage_tx_mutation_executor
	add_child(remove_seed_query_executor)
	add_child(remove_weed_query_executor)
	add_child(plant_seed_query_executor)
	add_child(harvest_seed_query_executor)
	add_child(stage_tx_mutation_executor)

	farms = SceneContext.user_state["villageState"]["houseFieldStates"]

	item_state_ids = SceneContext.user_state["inventoryState"]["itemStateList"]

	#create blank slots
	# Left slot
	for i in range(0,5):
		var farm
		if (farms[i] == null):
			farm = FarmSlotEmptyScn.instantiate()
			farm.im_left()
			farm.button_down.connect(farm_selected)
		else:
			if(farms[i]["isHarvested"]):
				farm = FarmSlotDoneScn.instantiate()
				farm.im_left()
				farm.set_farm_slot(farms[i])
				farm.button_down.connect(farm_selected)
				farm.button_down_name.connect(set_harvested_name)
				farm.button_down_harvest.connect(harvest_seed)
			else:
				farm = FarmSlotButtonScn.instantiate()
				farm.im_left()
				farm.button_down.connect(farm_selected)
				farm.button_down_action.connect(control_seed)
				farm.set_farm_slot(farms[i])
		
		left_farm.add_child(farm)
	
	# Right slot
	for i in range(5,10):
		var farm
		if (farms[i] == null):
			farm = FarmSlotEmptyScn.instantiate()
			farm.im_right()
			farm.button_down.connect(farm_selected)
		else:
			if(farms[i]["isHarvested"]):
				farm = FarmSlotDoneScn.instantiate()
				farm.im_right()
				farm.set_farm_slot(farms[i])
				farm.button_down.connect(farm_selected)
				farm.button_down_name.connect(set_harvested_name)
				farm.button_down_harvest.connect(harvest_seed)
			else:
				farm = FarmSlotButtonScn.instantiate()
				farm.im_right()
				farm.button_down.connect(farm_selected)
				farm.button_down_action.connect(control_seed)
				farm.set_farm_slot(farms[i])
		
		right_farm.add_child(farm)
	
	
func farm_selected(farm_index):
	var format_string = "farm selected: %s"
	SceneContext.selected_field_index = farm_index
	if (farms[farm_index] == null):
		plant_popup()
	#else:
		#if(farms[farm_index]["isHarvested"]):
			#done_popup()
		#else:
			#pass

func plant_popup():
	if is_instance_valid(popup_area):
		for child in popup_area.get_children():
			child.queue_free()
	
	var amount = item_state_ids.size()
	var mouse_pos = get_local_mouse_position() + Vector2(0, -200)
	var install_popup = FarmInstallPopupScn.instantiate()
	install_popup.set_amount(amount)
	popup_area.add_child(install_popup)
	install_popup.set_position(mouse_pos)
	install_popup.accept_button_down.connect(plant_seed)


func plant_seed():
	query_executor.stage_action(
		{
			"publicKey": GlobalSigner.signer.GetPublicKey(),
			"fieldIndex": SceneContext.selected_field_index,
			"itemStateIdToUse": item_state_ids[0]["stateID"],
		},
		plant_seed_query_executor,
		stage_tx_mutation_executor
	)

func set_harvested_name(seed_name):
	harvested_name = seed_name
	
func done_popup():
	if is_instance_valid(popup_area):
		for child in popup_area.get_children():
			child.queue_free()

	var done_popup = DoneNoticePopupScn.instantiate()
	done_popup.set_seed_name(harvested_name)
	popup_area.add_child(done_popup)
	# 팝업 위치는 설정이 필요할 듯
	done_popup.set_position(Vector2(700,500))

func harvest_seed():
	action_success = false
	
	query_executor.stage_action(
		{
			"publicKey": GlobalSigner.signer.GetPublicKey(),
			"fieldIndex": SceneContext.selected_field_index,
		},
		harvest_seed_query_executor,
		stage_tx_mutation_executor
	)

func action_popup(weed : bool):
	if is_instance_valid(popup_area):
		for child in popup_area.get_children():
			child.queue_free()
	
	var action_popup = FarmActionPopupScn.instantiate()
	action_popup.set_weed_button(weed)
	popup_area.add_child(action_popup)
	var mouse_pos = get_local_mouse_position() + Vector2(0, -200)
	action_popup.set_position(mouse_pos)
	action_popup.button_down_remove.connect(remove_popup)
	action_popup.weed_action_signal.connect(remove_weed)
	
func remove_popup():
	if is_instance_valid(popup_area):
		for child in popup_area.get_children():
			child.queue_free()
	
	var remove_popup = FarmAskRemovePopupScn.instantiate()
	popup_area.add_child(remove_popup)
	remove_popup.set_position(Vector2(700,500))
	remove_popup.button_yes.connect(remove_done_popup)

func remove_done_popup():
	remove_seed()
	if is_instance_valid(popup_area):
		for child in popup_area.get_children():
			child.queue_free()
	
	var done_popup = FarmRemoveDonePopupScn.instantiate()
	popup_area.add_child(done_popup)
	done_popup.set_position(Vector2(700,500))
	done_popup.refresh_me.connect(fetch_new)

func control_seed(weed : bool):
	#code here
	action_popup(weed)

func remove_seed():
	query_executor.stage_action(
		{
			"publicKey": GlobalSigner.signer.GetPublicKey(),
			"fieldIndex": SceneContext.selected_field_index,
		},
		remove_seed_query_executor,
		stage_tx_mutation_executor
	)
	
	fetch_new()

func fetch_new():
# fetch datas
	Intro._query_user_state()
	Intro.get_current_block()
	
	# done popup
	if(action_success):
		done_popup()
		action_success = false
	
# delete old farm uis
	if is_instance_valid(left_farm):
		for child in left_farm.get_children():
			child.queue_free()
	if is_instance_valid(right_farm):
		for child in right_farm.get_children():
			child.queue_free()
	
	_ready()

func remove_weed():
	query_executor.stage_action(
		{
			"publicKey": GlobalSigner.signer.GetPublicKey(),
			"fieldIndex": SceneContext.selected_field_index,
		},
		remove_weed_query_executor,
		stage_tx_mutation_executor
	)

func _on_refresh_button_down():
	fetch_new()

func _on_home_button_down():
	get_tree().change_scene_to_file("res://scenes/house/house.tscn")

func _on_market_button_down():
	get_tree().change_scene_to_file("res://scenes/market/market.tscn")
