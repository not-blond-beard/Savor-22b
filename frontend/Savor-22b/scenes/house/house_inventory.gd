extends Control

const KitchenToolsScn = preload("res://scenes/house/kitchen_tools.tscn")
const KitchenShopScn = preload("res://scenes/house/kitchen_shop.tscn")
const Refrigerator = preload("res://scenes/house/refrigerator.tscn")

@onready var panel = $M/V/Panel/C

signal buy_signal
signal close_all

func _ready():
	_on_tools_button_down()

func _on_tools_button_down():
	clear_popup()
	var kitchens = KitchenToolsScn.instantiate()
	kitchens.close_tab.connect(close_tab)
	panel.add_child(kitchens)

func _on_shop_button_down():
	clear_popup()
	var kitchens = KitchenShopScn.instantiate()
	kitchens.close_tab.connect(close_tab)
	kitchens.buy_signal.connect(popup)
	panel.add_child(kitchens)
	
func clear_popup():
	if is_instance_valid(panel):
		for pop in panel.get_children():
			pop.queue_free()

func close_tab():
	close_all.emit()
	queue_free()

func popup():
	buy_signal.emit()

func _on_ingredients_button_down():
	clear_popup()
	var refrigerator = Refrigerator.instantiate()
	refrigerator.close_tab.connect(close_tab)
	panel.add_child(refrigerator)
