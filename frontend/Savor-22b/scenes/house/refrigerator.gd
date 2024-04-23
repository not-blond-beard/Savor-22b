extends Control

const FOOD = preload("res://scenes/house/food.tscn")

@onready var grid = $M/V/Items/S/G

signal closetab

var foods

func _ready():
	foods = SceneContext.user_state.inventoryState["refrigeratorStateList"]
	
	for food in foods:
		var foodpanel = FOOD.instantiate()
		foodpanel.set_info(food)
		grid.add_child(foodpanel)



func _on_close_button_down():
	closetab.emit()
	queue_free()

