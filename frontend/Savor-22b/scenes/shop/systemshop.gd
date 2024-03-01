extends Panel

const ITEM = preload("res://scenes/shop/shop_item.tscn")
const SHOP_ASK_POPUP = preload("res://scenes/shop/ask_popup.tscn")
const SHOP_DONE_POPUP = preload("res://scenes/shop/done_popup.tscn")

@onready var shoplist = $M/H/C/M/Lists
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
	
	popup.add_child(askpopup)
	

func _on_close_button_down():
	queue_free()
