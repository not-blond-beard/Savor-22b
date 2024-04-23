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
		if(items.isCooking && items.equipmentCategoryType != "main"):
			print(items)
			menus.append(items)
			
	refine_data()

func refine_data():
	for menu1 in menus:
		for big in SceneContext.installed_tool_info:
			print(big)
			if menu1["cookingFood"].stateId == big.foodId:
				menus.erase(menu1)
