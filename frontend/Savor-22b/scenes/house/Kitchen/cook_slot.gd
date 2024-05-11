extends Control

const CookMenuScn = preload("res://scenes/house/Kitchen/cook_menu.tscn")

@onready var slot = $P/M/V/S/Slot

var menus: Array
var large_slots: Dictionary
var large_arr: Array		#큰 조리도구의 메뉴

func _ready():
	load_data()
	
	for menu in menus:
		var single_menu = CookMenuScn.instantiate()
		single_menu.set_data(menu)
		slot.add_child(single_menu)
	
func load_data():
	var refrigerator = SceneContext.user_state["inventoryState"]["kitchenEquipmentStateList"]
	for items in refrigerator:
		if(items.isCooking && items.equipmentCategoryType != "main"):
			menus.append(items)
			
	refine_data()

func refine_data():
	for menu1 in menus:
		for big in SceneContext.installed_tool_info:
			if menu1["cookingFood"].stateId == big.foodId:
				menus.erase(menu1)
	large_slots = SceneContext.user_kitchen_state["villageState"]["houseState"]["kitchenState"]
	
	var slot1 = large_slots["firstApplianceSpace"]
	var slot2 = large_slots["secondApplianceSpace"]
	var slot3 = large_slots["thirdApplianceSpace"]
	
	if (slot1["installedKitchenEquipment"]!= null):
		var obj1 = slot1["installedKitchenEquipment"]["stateId"]
		large_arr.append(obj1)
	if (slot2["installedKitchenEquipment"] != null):
		var obj2 = slot2["installedKitchenEquipment"]["stateId"]
		large_arr.append(obj2)
	if (slot3["installedKitchenEquipment"] != null):
		var obj3 = slot3["installedKitchenEquipment"]["stateId"]
		large_arr.append(obj3)
			
	for menu in menus:
		for data in large_arr:
			if data == menu["stateId"]:
				menus.erase(menu)
	
