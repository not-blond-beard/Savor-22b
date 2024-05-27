extends Control

const TradeGoodsScn = preload("res://scenes/market/trade_goods.tscn")
const MyTradeGoodsScn = preload("res://scenes/market/my_trade_goods.tscn")

@onready var list_container = $MarginContainer/VBoxContainer/ListPanel/ScrollContainer/MarginContainer/CenterContainer/GridContainer

var goods
var my_address = GlobalSigner.signer_address

func _ready():
	pass

func set_list(data: Array):
	goods = data
	load_list()

func load_list():
	clear_list_panel()
	
	for good in goods:
		if(good["sellerAddress"] != my_address):
			add_lists(good, false)

func _on_my_sell_list_button_down():
	clear_list_panel()

	for good in goods:
		if(good["sellerAddress"] == my_address):
			add_lists(good, true)

func clear_list_panel():
	if is_instance_valid(list_container):
		for posted_item in list_container.get_children():
			posted_item.queue_free()
			
func add_lists(good: Dictionary, is_mine: bool):
	var trade_goods_scene
	if(!is_mine):
		trade_goods_scene = TradeGoodsScn.instantiate()
	else:
		trade_goods_scene = MyTradeGoodsScn.instantiate()
	trade_goods_scene.set_info(good)
	list_container.add_child(trade_goods_scene)

func _on_whole_sell_list_button_down():
	load_list()
