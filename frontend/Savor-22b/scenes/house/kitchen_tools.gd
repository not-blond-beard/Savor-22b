extends Control

const ToolScn = preload("res://scenes/house/tool.tscn")

@onready var grid = $M/V/Items/S/G

signal close_tab

var tools

func _ready():
	tools = SceneContext.user_state.inventoryState["kitchenEquipmentStateList"]
	
	for tool in tools:
		var tool_panel = ToolScn.instantiate()
		tool_panel.set_info(tool)
		grid.add_child(tool_panel)

func _on_close_button_down():
	close_tab.emit()
	queue_free()

