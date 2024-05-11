extends Control

signal install_signal
signal reload_signal

const BigToolScn = preload("res://scenes/house/big_tool.tscn")

@onready var slot = $B/M/V/S/G

var big_tools: Array
var selected_state_id

func _ready():
	load_tools()
	set_tools()

func load_tools():
	var kitchen_tools = SceneContext.user_state["inventoryState"]["kitchenEquipmentStateList"]
	for tool in kitchen_tools:
		if (tool.equipmentCategoryType == "main"):
			big_tools.append(tool)
		for used in SceneContext.installed_tool_id:
			if (tool.stateId == used):
				big_tools.erase(tool)

func set_tools():
	for tool in big_tools:
		var single_tool = BigToolScn.instantiate()
		single_tool.set_data(tool)
		single_tool.button_pressed.connect(get_stateId)
		slot.add_child(single_tool)
		
func get_stateId(stateId: String):
	selected_state_id = stateId

func _on_install_button_down():
	install_signal.emit(selected_state_id)
	reload_signal.emit()
	queue_free()

func _on_close_button_down():
	reload_signal.emit()
	queue_free()
