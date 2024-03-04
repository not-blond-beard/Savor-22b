extends Control

const KITCHEN_TOOLS = preload("res://scenes/house/kitchentools.tscn")

@onready var panel = $M/V/Panel/C

func _ready():
	var kitchens = KITCHEN_TOOLS.instantiate()
	
	panel.add_child(kitchens)

