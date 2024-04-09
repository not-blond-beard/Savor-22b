extends Control

const HOUSE_INVENTORY = preload("res://scenes/house/house_inventory.tscn")
const ASK_POPUP = preload("res://scenes/shop/ask_popup.tscn")
const DONE_POPUP = preload("res://scenes/shop/done_popup.tscn")

const RECIPE = preload("res://scenes/house/recipebook/recipebook.tscn")

const SMALL_COOK = preload("res://scenes/house/Kitchen/cook_slot.tscn")
const LARGE_COOK = preload("res://scenes/house/Kitchen/big_tool_slot.tscn")

const LARGE_INSTALLER = preload("res://scenes/house/bigtool_install_popup.tscn")

const Gql_query = preload("res://gql/query.gd")

@onready var subscene = $M/V/subscene
@onready var popup = $Popups

var selectedSpace

func _ready():
	load_kitchen()

func _on_inventory_button_button_down():
	clear_popup()
	reload_subscene()

	var inventory = HOUSE_INVENTORY.instantiate()
	inventory.buysignal.connect(buypopup)
	inventory.closeall.connect(clear_popup)
	subscene.add_child(inventory)

func buypopup():
	var askpopup = ASK_POPUP.instantiate()
	askpopup.set_itemname(SceneContext.selected_item_name)
	askpopup.buy_button_down.connect(buyaction)
	askpopup.set_position(Vector2(900,600))
	popup.add_child(askpopup)

func buyaction():
	clear_popup()
	buytool()
	
	var donepopup = DONE_POPUP.instantiate()
	donepopup.set_position(Vector2(900,600))
	popup.add_child(donepopup)

func buytool():
	var itemnum = SceneContext.selected_item_index
	var gql_query = Gql_query.new()
	var query_string = gql_query.buy_kitchen_equipment_query_format.format([
		"\"%s\"" % GlobalSigner.signer.GetPublicKey(),
		itemnum], "{}")
	print(query_string)
	
	var query_executor = SvrGqlClient.raw(query_string)
	query_executor.graphql_response.connect(func(data):
		print("gql response: ", data)
		var unsigned_tx = data["data"]["createAction_BuyKitchenEquipment"]
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

func clear_popup():
	if is_instance_valid(popup):
		for pop in popup.get_children():
			pop.queue_free()
	load_kitchen()

func reload_subscene():
	if is_instance_valid(subscene):
		for scene in subscene.get_children():
			scene.queue_free()
	Intro._query_user_state()
	Intro._query_kitchen_slot_state()


func _on_recipe_button_button_down():
	clear_popup()
	reload_subscene()
	var Recipebookarea = MarginContainer.new()
	#setting margincontainer constants
	Recipebookarea.add_theme_constant_override("margin_top", 20)
	Recipebookarea.add_theme_constant_override("margin_bottom", 20)
	Recipebookarea.add_theme_constant_override("margin_left", 140)
	Recipebookarea.add_theme_constant_override("margin_right", 140)
	subscene.add_child(Recipebookarea)

	var recipebook = RECIPE.instantiate()
	recipebook.closeall.connect(clear_popup)

	Recipebookarea.add_child(recipebook)

func _on_farm_button_button_down():
	get_tree().change_scene_to_file("res://scenes/farm.tscn")

func _on_village_button_button_down():
	get_tree().change_scene_to_file("res://scenes/village_view.tscn")

func _on_refresh_button_button_down():
	clear_popup()
	reload_subscene()
	load_kitchen()


func load_kitchen():
	reload_subscene()

	var Kitchenarea = VBoxContainer.new()
	Kitchenarea.add_theme_constant_override("separation", 20)
	subscene.add_child(Kitchenarea)
	
	var smallslot = SMALL_COOK.instantiate()
	Kitchenarea.add_child(smallslot)
	
	var largeslot = LARGE_COOK.instantiate()
	largeslot.install_signal.connect(on_empty_slot_pressed)
	Kitchenarea.add_child(largeslot)
	
func on_empty_slot_pressed(spaceNumber : int):
	
	var largeinstaller = LARGE_INSTALLER.instantiate()

	var installerarea = MarginContainer.new()
	#setting margincontainer constants
	installerarea.add_theme_constant_override("margin_top", 20)
	installerarea.add_theme_constant_override("margin_bottom", 320)
	installerarea.add_theme_constant_override("margin_left", 150)
	installerarea.add_theme_constant_override("margin_right", 150)
	subscene.add_child(installerarea)
	installerarea.add_child(largeinstaller)
	largeinstaller.install_signal.connect(install_tool)
	largeinstaller.reload_signal.connect(load_kitchen)
	
	selectedSpace = spaceNumber
	

func install_tool(stateId : String):
	var gql_query = Gql_query.new()
	var query_string = gql_query.install_kitchen_equipment_query_format.format([
		"\"%s\"" % GlobalSigner.signer.GetPublicKey(),
		"\"%s\"" % stateId, selectedSpace], "{}")
	print(query_string)
	
	var query_executor = SvrGqlClient.raw(query_string)
	query_executor.graphql_response.connect(func(data):
		print("gql response: ", data)
		var unsigned_tx = data["data"]["createAction_InstallKitchenEquipmentAction"]
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
