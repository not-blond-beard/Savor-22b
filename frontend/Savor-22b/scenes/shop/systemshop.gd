extends Panel

const ITEM = preload("res://scenes/shop/shop_item.tscn")
const SHOP_ASK_POPUP = preload("res://scenes/shop/ask_popup.tscn")
const SHOP_DONE_POPUP = preload("res://scenes/shop/done_popup.tscn")

const Gql_query = preload("res://gql/query.gd")

@onready var shoplist = $M/H/C/M/S/Lists
@onready var popup = $Popups

var shopitems = []



func _ready():
	print("shop opened")
	shopitems = SceneContext.shop
	print(shopitems)
	var size = shopitems.size()
	
	for item in shopitems["items"]:
		print(item)
		var singleitem = ITEM.instantiate()
		singleitem.set_item(item)
		singleitem.button_down.connect(buy_item)
		shoplist.add_child(singleitem)
		


func buy_item():
	print("buy item")
	var askpopup = SHOP_ASK_POPUP.instantiate()
	askpopup.buy_button_down.connect(buyquery)
	askpopup.set_itemname(SceneContext.selected_item_name)
	popup.add_child(askpopup)
	

func buyquery():
	var itemnum = SceneContext.selected_item_index
	var gql_query = Gql_query.new()
	var query_string = gql_query.buy_shop_item_query_format.format([
		"\"%s\"" % GlobalSigner.signer.GetPublicKey(),
		itemnum], "{}")
	print(query_string)
	
	var query_executor = SvrGqlClient.raw(query_string)
	query_executor.graphql_response.connect(func(data):
		print("gql response: ", data)
		var unsigned_tx = data["data"]["createAction_BuyShopItem"]
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
	
	print_done_popup()

func print_done_popup():
	clear_popup()
	var pop = SHOP_DONE_POPUP.instantiate()
	popup.add_child(pop)



func clear_popup():
	if is_instance_valid(popup):
		for pop in popup.get_children():
			pop.queue_free()

func _on_close_button_down():
	queue_free()
