extends Control

const MyItemScn = preload("res://scenes/market/my_item.tscn")

@onready var inventory_container = $MarginContainer/VBoxContainer/InventoryPanel/ScrollContainer/CenterContainer/GridContainer

var items

func _ready():
	load_items()

func load_items():
	items = SceneContext.user_state.inventoryState["refrigeratorStateList"]
	
	for item in items:
		var item_scene = MyItemScn.instantiate()
		item_scene.set_info(item)
		inventory_container.add_child(item_scene)
