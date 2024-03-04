extends Control

const KITCHEN_TOOLS = preload("res://scenes/house/kitchentools.tscn")
const KITCHEN_SHOP = preload("res://scenes/house/kitchenshop.tscn")

@onready var panel = $M/V/Panel/C

func _ready():
	var kitchens = KITCHEN_TOOLS.instantiate()
	
	panel.add_child(kitchens)




func _on_tools_button_down():
	clear_popup()
	var kitchens = KITCHEN_TOOLS.instantiate()
	
	panel.add_child(kitchens)

func _on_shop_button_down():
	clear_popup()
	var kitchens = KITCHEN_SHOP.instantiate()
	
	panel.add_child(kitchens)
	
func clear_popup():
	if is_instance_valid(panel):
		for pop in panel.get_children():
			pop.queue_free()
