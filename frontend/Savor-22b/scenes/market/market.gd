extends Control

signal query_received

const TradeInventoryScn = preload("res://scenes/market/trade_inventory.tscn")
const SellListScn = preload("res://scenes/market/sell_list.tscn")

@onready var inventory_container = $VBoxContainer/MarginContainer/SubMenuHBoxContainer/InventoryMarginContainer
@onready var sell_list_container = $VBoxContainer/MarginContainer/SubMenuHBoxContainer/SellListMarginContainer

var query_executor = QueryExecutor.new()
var register_trade_good_query_executor
var stage_tx_mutation_executor
var trade_inventory_state_executor

var inventory_state


func _ready():
	register_trade_good_query_executor = query_executor.register_trade_good_query_executor
	stage_tx_mutation_executor = query_executor.stage_tx_mutation_executor
	trade_inventory_state_executor = query_executor.trade_inventory_state_executor
	add_child(register_trade_good_query_executor)
	add_child(stage_tx_mutation_executor)
	add_child(trade_inventory_state_executor)
	
	
	load_initial_scene()

	print(inventory_state)


func load_initial_scene():
	load_inventory()
	
	query_trade_inventory_state()
	query_received.connect(load_sell_list)


func load_inventory():
	var inventory = TradeInventoryScn.instantiate()
	inventory_container.add_child(inventory)
	
func load_sell_list():
	print(inventory_state)
	if (inventory_state != null):
		var sell_list = SellListScn.instantiate()
		sell_list_container.add_child(sell_list)
		sell_list.set_list(inventory_state)
	




func test_register():
	var food_id = "0cffada3-dccf-48ee-942c-2617ac46952b"
	query_executor.stage_action(
		{
			"publicKey": GlobalSigner.signer.GetPublicKey(),
			"price": 150,
			"foodStateId": food_id,
			"itemStateIds": []
		},
		register_trade_good_query_executor,
		stage_tx_mutation_executor
	)

func query_action(query_executor): # query with no args
	query_executor.graphql_response.connect(
		func(data):
			inventory_state = data["data"]["tradeInventoryState"]["tradeGoods"]
			query_received.emit()
	)
	query_executor.run({})
	
func query_trade_inventory_state():
	query_action(
		trade_inventory_state_executor
	)

func _on_village_button_down():
	get_tree().change_scene_to_file("res://scenes/village/village_view.tscn")


func _on_test_button_down():
	test_register()
