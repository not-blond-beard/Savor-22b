extends Control

const SLOT_EMPTY = preload("res://scenes/house/Kitchen/tool_slot_empty.tscn")
const SLOT_NOT_USED = preload("res://scenes/house/Kitchen/tool_not_used.tscn")
const SLOT_USED = preload("res://scenes/house/Kitchen/tool_is_used.tscn")

@onready var slot = $P/M/V/Slot

var largeslots: Dictionary
var largetools: Array

func _ready():

	load_data()
	
#slot 1
	var slot1 = largeslots["firstApplianceSpace"]
	if (slot1.installedKitchenEquipment == null): # not installed
		var bigslot = SLOT_EMPTY.instantiate()
		slot.add_child(bigslot)
	else: # installed but not used
		if (!slot1["installedKitchenEquipment"]["isCooking"]):
			var bigslot = SLOT_NOT_USED.instantiate()
			bigslot.set_data(slot1)
			slot.add_child(bigslot)
		else: # cooking
			var bigslot = SLOT_USED.instantiate()
			bigslot.set_data(slot1)
			slot.add_child(bigslot)
#slot 2
	var slot2 = largeslots["secondApplianceSpace"]
	if (slot2.installedKitchenEquipment == null):
		var bigslot = SLOT_EMPTY.instantiate()
		slot.add_child(bigslot)
	else:
		if (!slot2["installedKitchenEquipment"]["isCooking"]):
			var bigslot = SLOT_NOT_USED.instantiate()
			bigslot.set_data(slot2)
			slot.add_child(bigslot)
		else:
			var bigslot = SLOT_USED.instantiate()
			bigslot.set_data(slot2)
			slot.add_child(bigslot)
#slot 3
	var slot3 = largeslots["thirdApplianceSpace"]
	if (slot3.installedKitchenEquipment == null):
		var bigslot = SLOT_EMPTY.instantiate()
		slot.add_child(bigslot)
	else:
		if (!slot3["installedKitchenEquipment"]["isCooking"]):
			var bigslot = SLOT_NOT_USED.instantiate()
			bigslot.set_data(slot3)
			slot.add_child(bigslot)
		else:
			var bigslot = SLOT_USED.instantiate()
			bigslot.set_data(slot3)
			slot.add_child(bigslot)

	
func load_data():
	largeslots = SceneContext.user_kitchen_state["villageState"]["houseState"]["kitchenState"]
	
	var tools = SceneContext.user_state["inventoryState"]["kitchenEquipmentStateList"]
	for tool in tools:
		if(tool.equipmentCategoryType == "main"):
			largetools.append(tool)

