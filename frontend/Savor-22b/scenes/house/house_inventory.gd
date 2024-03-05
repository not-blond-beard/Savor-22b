extends Control

const KITCHEN_TOOLS = preload("res://scenes/house/kitchentools.tscn")
const KITCHEN_SHOP = preload("res://scenes/house/kitchenshop.tscn")

@onready var panel = $M/V/Panel/C

signal buysignal

func _ready():
	var kitchens = KITCHEN_TOOLS.instantiate()
	
	panel.add_child(kitchens)




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
	queue_free()

func popup():
	buysignal.emit()
