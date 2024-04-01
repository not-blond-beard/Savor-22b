extends Control

const MENU = preload("res://scenes/house/Kitchen/cook_menu.tscn")

@onready var slot = $P/M/V/S/Slot

var menus: Array
var largeslots: Dictionary
var largearr: Array		#큰 조리도구의 메뉴

func _ready():
	load_data();
	
	for menu in menus:
		var singlemenu = MENU.instantiate()
		singlemenu.set_data(menu)
		slot.add_child(singlemenu)
	
	
func load_data():
	var refrigerator = SceneContext.user_state["inventoryState"]["kitchenEquipmentStateList"]
	for items in refrigerator:
		if(items.isCooking):
			menus.append(items)
	
	refine_data()


	
# 큰 조리도구의 메뉴를 제외시킨다
func refine_data():
	largeslots = SceneContext.user_kitchen_state["villageState"]["houseState"]["kitchenState"]
	#print(largeslots)
	
	var slot1 = largeslots["firstApplianceSpace"]
	var slot2 = largeslots["secondApplianceSpace"]
	var slot3 = largeslots["thirdApplianceSpace"]
	
	if (slot1["installedKitchenEquipment"]!= null):
		var obj1 = slot1["installedKitchenEquipment"]["stateId"]
		largearr.append(obj1)
	if (slot2["installedKitchenEquipment"] != null):
		var obj2 = slot2["installedKitchenEquipment"]["stateId"]
		largearr.append(obj2)
	if (slot3["installedKitchenEquipment"] != null):
		var obj3 = slot3["installedKitchenEquipment"]["stateId"]
		largearr.append(obj3)
			
	print(largearr)
	
	for menu in menus:
		for data in largearr:
			if data == menu["stateId"]:
				menus.erase(menu)
	
