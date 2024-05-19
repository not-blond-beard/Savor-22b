extends Panel

const ShopItemScn = preload("res://scenes/shop/shop_item.tscn")
const AskPopupScn = preload("res://scenes/shop/ask_popup.tscn")
const DonePopupScn = preload("res://scenes/shop/done_popup.tscn")

const GqlQueryExecutor = preload("res://gql/query_executor.gd")

@onready var shop_list = $M/H/C/M/S/Lists
@onready var popup = $Popups

var shop_items = []

var query_executor = QueryExecutor.new()
var buy_shop_item_query_executor
var stage_tx_mutation_executor

func _ready():
	buy_shop_item_query_executor = query_executor.buy_shop_item_query_executor
	stage_tx_mutation_executor = query_executor.stage_tx_mutation_executor
	add_child(buy_shop_item_query_executor)
	add_child(stage_tx_mutation_executor)

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
	
	query_executor.stage_action(
		{
			"publicKey": GlobalSigner.signer.GetPublicKey(),
			"desiredShopItemID": item_num,
		},
		buy_shop_item_query_executor,
		stage_tx_mutation_executor
	)
	
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
