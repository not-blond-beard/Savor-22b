extends Control

const TOOL = preload("res://scenes/house/tool.tscn")

@onready var grid = $M/V/Items/G

var list

func _ready():
	list = SceneContext.shop["kitchenEquipments"]
	
	for tool in list:
		var toolpanel = TOOL.instantiate()
		toolpanel.set_slottype()
		toolpanel.set_info(tool)
		grid.add_child(toolpanel)

