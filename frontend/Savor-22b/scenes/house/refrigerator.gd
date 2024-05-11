extends Control

const FoodScn = preload("res://scenes/house/food.tscn")

@onready var grid = $M/V/Items/S/G

signal close_tab

var foods

func _ready():
	foods = SceneContext.user_state.inventoryState["refrigeratorStateList"]
	
	for food in foods:
		var food_panel = FoodScn.instantiate()
		food_panel.set_info(food)
		grid.add_child(food_panel)

func _on_close_button_down():
	close_tab.emit()
	queue_free()

