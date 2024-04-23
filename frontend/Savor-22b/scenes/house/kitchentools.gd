extends Control

const TOOL = preload("res://scenes/house/tool.tscn")

@onready var grid = $M/V/Items/S/G

signal closetab

var tools

func _ready():
	tools = SceneContext.user_state.inventoryState["kitchenEquipmentStateList"]
	
	for tool in tools:
		var toolpanel = TOOL.instantiate()
		toolpanel.set_info(tool)
		grid.add_child(toolpanel)



func _on_close_button_down():
	closetab.emit()
	queue_free()

