extends Control

signal install_signal
signal uninstall_signal

const ToolSlotEmptyScn = preload("res://scenes/house/kitchen/tool_slot_empty.tscn")
const ToolNotUsedScn = preload("res://scenes/house/kitchen/tool_not_used.tscn")
const ToolIsUsedScn = preload("res://scenes/house/kitchen/tool_is_used.tscn")

@onready var slot = $P/M/V/Slot

var large_slots: Dictionary
var large_tools: Array
var installed_id: Array
var installed_name: Array
var installed_tools_info: Array
var selected_space: int

func _ready():
	load_data()

# Setting Slots
	set_slot("first")
	set_slot("second")
	set_slot("third")
	
	SceneContext.installed_tool_id = installed_id
	SceneContext.installed_tool_name = installed_name
	SceneContext.installed_tool_info = installed_tools_info

func load_data():
	large_slots = SceneContext.user_kitchen_state["villageState"]["houseState"]["kitchenState"]
	
	var tools = SceneContext.user_state["inventoryState"]["kitchenEquipmentStateList"]
	for tool in tools:
		if(tool.equipmentCategoryType == "main"):
			large_tools.append(tool)

func set_slot(name : String):
	var loc = "%s%s" % [name,"ApplianceSpace"]
	var single_slot = large_slots[loc]
	if (single_slot.installedKitchenEquipment == null): # not installed
		var big_slot = ToolSlotEmptyScn.instantiate()
		big_slot.install_tools.connect(on_signal_received)
		big_slot.set_data(single_slot)
		slot.add_child(big_slot)
	else: # installed but not used
		if (!single_slot["installedKitchenEquipment"]["isCooking"]):
			var big_slot = ToolNotUsedScn.instantiate()
			big_slot.set_data(single_slot)
			slot.add_child(big_slot)
			big_slot.uninstall_big_tool_button_pressed.connect(on_uninstall_signal_received)
			installed_id.append(single_slot["installedKitchenEquipment"]["stateId"])
			installed_name.append(single_slot["installedKitchenEquipment"]["equipmentName"])
			var dict = { "name" : single_slot["installedKitchenEquipment"]["equipmentName"],
				"stateId" : single_slot["installedKitchenEquipment"]["stateId"],
			"isCooking" : single_slot["installedKitchenEquipment"]["isCooking"],
			"foodId" : null}
			
			installed_tools_info.append(dict)
		else: # cooking
			var big_slot = ToolIsUsedScn.instantiate()
			big_slot.set_data(single_slot)
			slot.add_child(big_slot)
			var dict = { "name" : single_slot["installedKitchenEquipment"]["equipmentName"],
				"stateId" : single_slot["installedKitchenEquipment"]["stateId"],
			"isCooking" : single_slot["installedKitchenEquipment"]["isCooking"],
			"foodId" : single_slot["installedKitchenEquipment"]["cookingFood"]["stateId"]}
			
			installed_tools_info.append(dict)
			
func on_signal_received(spaceNumber : int):
	install_signal.emit(spaceNumber)

func on_uninstall_signal_received(spaceNumber : int):
	uninstall_signal.emit(spaceNumber)
