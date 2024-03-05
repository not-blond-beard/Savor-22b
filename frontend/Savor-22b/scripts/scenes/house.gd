extends Control

const HOUSE_INVENTORY = preload("res://scenes/house/house_inventory.tscn")
const ASK_POPUP = preload("res://scenes/shop/ask_popup.tscn")
const DONE_POPUP = preload("res://scenes/shop/done_popup.tscn")
const Gql_query = preload("res://gql/query.gd")

@onready var subscene = $M/V/subscene
@onready var popup = $Popups

func _ready():
	pass # Replace with function body.




func _on_inventory_button_button_down():
	var inventory = HOUSE_INVENTORY.instantiate()
	inventory.buysignal.connect(buypopup)
	subscene.add_child(inventory)


func buypopup():
	var askpopup = ASK_POPUP.instantiate()
	askpopup.set_itemname(SceneContext.selected_item_name)
	askpopup.buy_button_down.connect(buyaction)
	popup.add_child(askpopup)

func buyaction():
	clear_popup()
	buytool()
	
	var donepopup = DONE_POPUP.instantiate()
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
