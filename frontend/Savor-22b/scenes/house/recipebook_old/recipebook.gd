extends Control

const RECIPE = preload("res://scenes/house/recipebook/recipe.tscn")

@onready var grid = $background/M/V/S/G

var recipelist = SceneContext.recipe["recipe"]

func _ready():
	for singlerecipe in recipelist:
		var recipe = RECIPE.instantiate()
		recipe.set_info(singlerecipe)
		grid.add_child(recipe)




func _on_close_button_down():
	queue_free()
