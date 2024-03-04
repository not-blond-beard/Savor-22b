extends Control

const TOOL = preload("res://scenes/house/tool.tscn")

@onready var grid = $M/V/Items/G

var tools

func _ready():
	tools = SceneContext.user_state.inventoryState["kitchenEquipmentStateList"]
	
	for tool in tools:
		var toolpanel = TOOL.instantiate()
		toolpanel.set_info(tool)
		grid.add_child(toolpanel)

