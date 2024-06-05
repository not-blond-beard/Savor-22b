extends Control

const HouseInventoryScn = preload("res://scenes/house/house_inventory.tscn")
const AskPopupScn = preload("res://scenes/shop/ask_popup.tscn")
const DonePopupScn = preload("res://scenes/shop/done_popup.tscn")
const RecipeBookScn = preload("res://scenes/house/recipe_book/recipe_book.tscn")
const CookSlogScn = preload("res://scenes/house/kitchen/cook_slot.tscn")
const BigToolSlotScn = preload("res://scenes/house/kitchen/big_tool_slot.tscn")
const BigToolInstallPopupScn = preload("res://scenes/house/big_tool_install_popup.tscn")
const CookBookScn = preload("res://scenes/house/Cook/cook_book.tscn")
const CookStartedPopup = preload("res://scenes/house/Cook/cook_started_popup.tscn")
const ConfirmPopupScn = preload("res://scenes/common/prefabs/confirm_popup.tscn")

@onready var sub_scene = $M/V/sub_scene
@onready var popup = $Popups
@onready var confirmPopup = $ConfirmPopup

var selected_space
var cook_book

var query_executor = QueryExecutor.new()
var buy_kitchen_equipment_query_executor
var install_kitchen_equipment_query_executor
var uninstall_kitchen_equipment_query_executor
var stage_tx_mutation_executor

func _ready():
	buy_kitchen_equipment_query_executor = query_executor.buy_kitchen_equipment_query_executor
	install_kitchen_equipment_query_executor = query_executor.install_kitchen_equipment_query_executor
	uninstall_kitchen_equipment_query_executor = query_executor.uninstall_kitchen_equipment_query_executor
	stage_tx_mutation_executor = query_executor.stage_tx_mutation_executor
	add_child(buy_kitchen_equipment_query_executor)
	add_child(install_kitchen_equipment_query_executor)
	add_child(uninstall_kitchen_equipment_query_executor)
	add_child(stage_tx_mutation_executor)

	load_kitchen()

func _on_inventory_button_down():
	clear_popup()
	reload_sub_scene()

	var inventory = HouseInventoryScn.instantiate()
	inventory.buy_signal.connect(buy_popup)
	inventory.close_all.connect(clear_popup)
	sub_scene.add_child(inventory)

func buy_popup():
	var ask_popup = AskPopupScn.instantiate()
	ask_popup.set_item_name(SceneContext.selected_item_name)
	ask_popup.buy_button_down.connect(buy_action)
	ask_popup.set_position(Vector2(900,600))
	popup.add_child(ask_popup)

func buy_action():
	clear_popup()
	buy_tool()
	
	var done_popup = DonePopupScn.instantiate()
	done_popup.set_position(Vector2(900,600))
	popup.add_child(done_popup)

func buy_tool():
	var item_num = SceneContext.selected_item_index	
	
	query_executor.stage_action(
		{
			"publicKey": GlobalSigner.signer.GetPublicKey(),
			"desiredEquipmentID": item_num,
		},
		buy_kitchen_equipment_query_executor,
		stage_tx_mutation_executor
	)

func clear_popup():
	if is_instance_valid(popup):
		for pop in popup.get_children():
			pop.queue_free()
	load_kitchen()

func reload_sub_scene():
	if is_instance_valid(sub_scene):
		for scene in sub_scene.get_children():
			scene.queue_free()
	Intro._query_user_state()
	Intro._query_kitchen_slot_state()

func _on_recipe_button_down():
	clear_popup()
	reload_sub_scene()
	var recipe_book_area = MarginContainer.new()
	#setting margincontainer constants
	recipe_book_area.add_theme_constant_override("margin_top", 20)
	recipe_book_area.add_theme_constant_override("margin_bottom", 20)
	recipe_book_area.add_theme_constant_override("margin_left", 140)
	recipe_book_area.add_theme_constant_override("margin_right", 140)
	sub_scene.add_child(recipe_book_area)

	var recipe_book = RecipeBookScn.instantiate()
	recipe_book.close_all.connect(clear_popup)

	recipe_book_area.add_child(recipe_book)

func _on_farm_button_down():
	get_tree().change_scene_to_file("res://scenes/farm/farm.tscn")

func _on_village_button_down():
	get_tree().change_scene_to_file("res://scenes/village/village_view.tscn")

func _on_market_button_down():
	get_tree().change_scene_to_file("res://scenes/market/market.tscn")

func _on_refresh_button_down():
	clear_popup()
	reload_sub_scene()
	load_kitchen()

func load_kitchen():
	reload_sub_scene()

	var kitchen_area = VBoxContainer.new()
	kitchen_area.add_theme_constant_override("separation", 20)
	sub_scene.add_child(kitchen_area)
	
	var small_slot = CookSlogScn.instantiate()
	kitchen_area.add_child(small_slot)
	
	var large_slot = BigToolSlotScn.instantiate()
	large_slot.install_signal.connect(on_empty_slot_pressed)
	large_slot.uninstall_signal.connect(on_uninstall_slot_pressed)
	kitchen_area.add_child(large_slot)
	
func on_empty_slot_pressed(spaceNumber : int):
	var large_installer = BigToolInstallPopupScn.instantiate()

	var installer_area = MarginContainer.new()

	installer_area.add_theme_constant_override("margin_top", 20)
	installer_area.add_theme_constant_override("margin_bottom", 320)
	installer_area.add_theme_constant_override("margin_left", 150)
	installer_area.add_theme_constant_override("margin_right", 150)
	sub_scene.add_child(installer_area)
	installer_area.add_child(large_installer)
	large_installer.install_signal.connect(install_tool)
	large_installer.reload_signal.connect(load_kitchen)
	
	selected_space = spaceNumber

func install_tool(stateId : String):
	query_executor.stage_action(
		{
			"publicKey": GlobalSigner.signer.GetPublicKey(),
			"kitchenEquipmentStateID": stateId,
			"spaceNumber": selected_space,
		},
		install_kitchen_equipment_query_executor,
		stage_tx_mutation_executor
	)

func _on_cook_button_down():
	var cook_book_area = MarginContainer.new()
	#setting margincontainer constants
	cook_book_area.add_theme_constant_override("margin_top", 20)
	cook_book_area.add_theme_constant_override("margin_bottom", 20)
	cook_book_area.add_theme_constant_override("margin_left", 140)
	cook_book_area.add_theme_constant_override("margin_right", 140)
	sub_scene.add_child(cook_book_area)

	cook_book = CookBookScn.instantiate()
	cook_book.close_all.connect(clear_popup)
	cook_book.reload_signal.connect(reload_cookbook)
	cook_book.cook_started.connect(cook_started_popup)

	cook_book_area.add_child(cook_book)

func reload_cookbook():
	clear_popup()
	_on_cook_button_down()
	
func cook_started_popup():
	var start_popup = CookStartedPopup.instantiate()
	start_popup.close_book.connect(_on_refresh_button_down)
	start_popup.set_position(Vector2(900,600))
	sub_scene.add_child(start_popup)

func on_uninstall_slot_pressed(spaceNumber: int):
	var confirmPopupResource = ConfirmPopupScn.instantiate()
	
	confirmPopupResource.set_label("설치된 조리도구를 제거하시겠습니까?")
	confirmPopupResource.ok_button_clicked_signal.connect(uninsatll_big_tool.bind(spaceNumber))
	confirmPopup.add_child(confirmPopupResource)

func uninsatll_big_tool(spaceNumber: int):
	query_executor.stage_action(
		{
			"publicKey": GlobalSigner.signer.GetPublicKey(),
			"spaceNumber": spaceNumber,
		},
		uninstall_kitchen_equipment_query_executor,
		stage_tx_mutation_executor
	)
