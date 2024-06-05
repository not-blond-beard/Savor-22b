extends Control

signal query_received

const InventoryScn = preload("res://scenes/market/inventory.tscn")
const SellListScn = preload("res://scenes/market/trade_inventory.tscn")

@onready var inventory_container = $VBoxContainer/MarginContainer/SubMenuHBoxContainer/InventoryMarginContainer
@onready var trade_inventory_container = $VBoxContainer/MarginContainer/SubMenuHBoxContainer/SellListMarginContainer

var query_executor = QueryExecutor.new()
var stage_tx_mutation_executor
var trade_inventory_state_executor

var inventory_state

func _ready():
	stage_tx_mutation_executor = query_executor.stage_tx_mutation_executor
	trade_inventory_state_executor = query_executor.trade_inventory_state_executor
	add_child(stage_tx_mutation_executor)
	add_child(trade_inventory_state_executor)
	
	load_initial_scene()

func load_initial_scene():
	load_inventory()
	
	query_trade_inventory_state()
	query_received.connect(load_trade_inventory)

func load_inventory():
	var inventory = InventoryScn.instantiate()
	inventory_container.add_child(inventory)
	
func load_trade_inventory():
	if (inventory_state != null):
		var trade_inventory = SellListScn.instantiate()
		trade_inventory_container.add_child(trade_inventory)
		trade_inventory.set_list(inventory_state)

func props_only_query_action(query_executor): # query with no args
	query_executor.graphql_response.connect(
		func(data):
			inventory_state = data["data"]["tradeInventoryState"]["tradeGoods"]
			query_received.emit()
	)
	query_executor.run({})
	
func query_trade_inventory_state():
	props_only_query_action(
		trade_inventory_state_executor
	)

func _on_village_button_down():
	get_tree().change_scene_to_file("res://scenes/village/village_view.tscn")
