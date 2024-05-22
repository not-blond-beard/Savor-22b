extends Control

const TradeInventoryScn = preload("res://scenes/market/trade_inventory.tscn")
const SellListScn = preload("res://scenes/market/sell_list.tscn")

@onready var inventory_container = $VBoxContainer/MarginContainer/SubMenuHBoxContainer/InventoryMarginContainer
@onready var sell_list_container = $VBoxContainer/MarginContainer/SubMenuHBoxContainer/SellListMarginContainer

var query_executor = QueryExecutor.new()
var register_trade_good_query_executor
var stage_tx_mutation_executor

func _ready():
	register_trade_good_query_executor = query_executor.register_trade_good_query_executor
	stage_tx_mutation_executor = query_executor.stage_tx_mutation_executor
	add_child(register_trade_good_query_executor)
	add_child(stage_tx_mutation_executor)
	
	
	load_initial_scene()
	
	test_register()


func load_initial_scene():
	load_inventory()
	load_sell_list()

func load_inventory():
	var inventory = TradeInventoryScn.instantiate()
	inventory_container.add_child(inventory)
	
func load_sell_list():
	var sell_list = SellListScn.instantiate()
	sell_list_container.add_child(sell_list)
	

func test_register():
	query_executor.stage_action(
		{
			"publicKey": GlobalSigner.signer.GetPublicKey(),
			"price": 10,
			"foodStateId": "8ef2aa23-3595-4036-9ee9-7802d4663891",
			"itemStateIds": []
		},
		register_trade_good_query_executor,
		stage_tx_mutation_executor
	)


func _on_village_button_down():
	get_tree().change_scene_to_file("res://scenes/village/village_view.tscn")
