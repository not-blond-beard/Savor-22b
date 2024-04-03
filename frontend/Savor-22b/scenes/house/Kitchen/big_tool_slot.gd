extends Control

const SLOT_EMPTY = preload("res://scenes/house/Kitchen/tool_slot_empty.tscn")
const SLOT_NOT_USED = preload("res://scenes/house/Kitchen/tool_not_used.tscn")
const SLOT_USED = preload("res://scenes/house/Kitchen/tool_is_used.tscn")

@onready var slot = $P/M/V/Slot

var largeslots: Dictionary
var largetools: Array

func _ready():

	load_data()

# Setting Slots
	set_slot("first")
	set_slot("second")
	set_slot("third")

	
func load_data():
	largeslots = SceneContext.user_kitchen_state["villageState"]["houseState"]["kitchenState"]
	
	var tools = SceneContext.user_state["inventoryState"]["kitchenEquipmentStateList"]
	for tool in tools:
		if(tool.equipmentCategoryType == "main"):
			largetools.append(tool)

func set_slot(name : String):
	var loc = "%s%s" % [name,"ApplianceSpace"]
	var singleslot = largeslots[loc]
	if (singleslot.installedKitchenEquipment == null): # not installed
		var bigslot = SLOT_EMPTY.instantiate()
		slot.add_child(bigslot)
	else: # installed but not used
		if (!singleslot["installedKitchenEquipment"]["isCooking"]):
			var bigslot = SLOT_NOT_USED.instantiate()
			bigslot.set_data(singleslot)
			slot.add_child(bigslot)
		else: # cooking
			var bigslot = SLOT_USED.instantiate()
			bigslot.set_data(singleslot)
			slot.add_child(bigslot)
