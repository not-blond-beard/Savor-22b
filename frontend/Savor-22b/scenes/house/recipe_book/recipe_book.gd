extends Control

signal close_all

const RecipeScn = preload("res://scenes/house/recipe_book/recipe.tscn")

@onready var grid = $background/M/V/S/G

var recipe_list = SceneContext.recipe["recipe"]

func _ready():
	for single_recipe in recipe_list:
		var recipe = RecipeScn.instantiate()
		recipe.set_info(single_recipe)
		grid.add_child(recipe)

func _on_close_button_down():
	queue_free()
	close_all.emit()
