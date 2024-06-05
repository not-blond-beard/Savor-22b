extends Control

const MyItemScn = preload("res://scenes/market/my_item.tscn")

@onready var inventory_container = $MarginContainer/VBoxContainer/InventoryPanel/ScrollContainer/CenterContainer/MarginContainer/GridContainer

var items
var grouped_items = {}

func _ready():
	load_items()


func load_items():
	items = SceneContext.user_state.inventoryState["refrigeratorStateList"]
	sort_inventory_by_items()
	
	for item in grouped_items.values():
		for grade in item.values():
			var item_scene = MyItemScn.instantiate()
			item_scene.set_info(grade)
			inventory_container.add_child(item_scene)

func sort_inventory_by_items():
	for item in items:
		var name = item["name"]
		var grade = item["grade"]
			
			# 이름이 존재하지 않으면 새로 추가
		if not grouped_items.has(name):
			grouped_items[name] = {}
			
			# 등급이 존재하지 않으면 이름 하위에 새로 추가
		if not grouped_items[name].has(grade):
			grouped_items[name][grade] = []
			
			# 아이템을 등급 배열에 추가
		grouped_items[name][grade].append(item)
