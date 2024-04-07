extends Control

signal install_signal
signal reload_signal

const BIGTOOL = preload("res://scenes/house/bigtool.tscn")

@onready var slot = $B/M/V/S/G

var bigtools: Array
var selectedStateId

func _ready():
	load_tools()
	set_tools()


func load_tools():
	var kitchentools = SceneContext.user_state["inventoryState"]["kitchenEquipmentStateList"]
	for tool in kitchentools:
		if (tool.equipmentCategoryType == "main"):
			bigtools.append(tool)
		for used in SceneContext.installed_tool_id:
			if (tool.stateId == used):
				bigtools.erase(tool)

func set_tools():
	for tool in bigtools:
		var singletool = BIGTOOL.instantiate()
		singletool.set_data(tool)
		singletool.button_pressed.connect(get_stateId)
		slot.add_child(singletool)
		

func get_stateId(stateId: String):
	selectedStateId = stateId


func _on_install_button_button_down():
	install_signal.emit(selectedStateId)
	reload_signal.emit()
	queue_free()


func _on_close_button_button_down():
	reload_signal.emit()
	queue_free()
