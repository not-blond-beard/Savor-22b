extends Control

const ToolScn = preload("res://scenes/house/tool.tscn")

@onready var grid = $M/V/Items/G

signal buy_signal
signal close_tab

var list

func _ready():
	list = SceneContext.shop["kitchenEquipments"]
	
	for tool in list:
		var tool_panel = ToolScn.instantiate()
		tool_panel.set_slot_type()
		tool_panel.set_info(tool)
		tool_panel.buy_signal.connect(popup)
		grid.add_child(tool_panel)

func popup():
	buy_signal.emit()

func _on_close_button_down():
	close_tab.emit()
	queue_free()
