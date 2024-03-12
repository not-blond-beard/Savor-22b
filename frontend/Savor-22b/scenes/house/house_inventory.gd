extends Control

const KITCHEN_TOOLS = preload("res://scenes/house/kitchentools.tscn")
const KITCHEN_SHOP = preload("res://scenes/house/kitchenshop.tscn")

@onready var panel = $M/V/Panel/C

signal buysignal
signal closeall

func _ready():
	_on_tools_button_down()




func _on_tools_button_down():
	clear_popup()
	var kitchens = KITCHEN_TOOLS.instantiate()
	kitchens.closetab.connect(closetab)
	panel.add_child(kitchens)

func _on_shop_button_down():
	clear_popup()
	var kitchens = KITCHEN_SHOP.instantiate()
	kitchens.closetab.connect(closetab)
	kitchens.buysignal.connect(popup)
	panel.add_child(kitchens)
	
func clear_popup():
	if is_instance_valid(panel):
		for pop in panel.get_children():
			pop.queue_free()

func closetab():
	closeall.emit()
	queue_free()

func popup():
	buysignal.emit()
