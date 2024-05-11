extends Panel

const ShopItemScn = preload("res://scenes/shop/shop_item.tscn")
const AskPopupScn = preload("res://scenes/shop/ask_popup.tscn")
const DonePopupScn = preload("res://scenes/shop/done_popup.tscn")

const GqlQuery = preload("res://gql/query.gd")

@onready var shop_list = $M/H/C/M/S/Lists
@onready var popup = $Popups

var shop_items = []

func _ready():
	shop_items = SceneContext.shop
	var size = shop_items.size()
	
	for item in shop_items["items"]:
		var single_item = ShopItemScn.instantiate()
		single_item.set_item(item)
		single_item.button_down.connect(buy_item)
		shop_list.add_child(single_item)

func buy_item():
	var ask_popup = AskPopupScn.instantiate()
	ask_popup.buy_button_down.connect(buy_query)
	ask_popup.set_item_name(SceneContext.selected_item_name)
	popup.add_child(ask_popup)

func buy_query():
	var item_num = SceneContext.selected_item_index
	var gql_query = GqlQuery.new()
	var query_string = gql_query.buy_shop_item_query_format.format([
		"\"%s\"" % GlobalSigner.signer.GetPublicKey(),
		item_num], "{}")
		
	var query_executor = SvrGqlClient.raw(query_string)
	query_executor.graphql_response.connect(
		func(data):
			var unsigned_tx = data["data"]["createAction_BuyShopItem"]
			var signature = GlobalSigner.sign(unsigned_tx)
			var mutation_executor = SvrGqlClient.raw_mutation(gql_query.stage_tx_query_format % [unsigned_tx, signature])
			add_child(mutation_executor)
			mutation_executor.run({})
	)
	add_child(query_executor)
	query_executor.run({})
	
func print_done_popup():
	clear_popup()
	var pop = DonePopupScn.instantiate()
	popup.add_child(pop)

func clear_popup():
	if is_instance_valid(popup):
		for pop in popup.get_children():
			pop.queue_free()

func _on_close_button_down():
	queue_free()
