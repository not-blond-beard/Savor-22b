extends Control

const IngredientScn = preload("res://scenes/house/recipe_book/ingredient.tscn")

@onready var ingredients = $panel/M/V/Description/Ingredients/list
@onready var tools = $panel/M/V/Description/Tools
@onready var req_block = $panel/M/V/Blockreq

var info
var name_format = "[%s] 레시피"
var block_format = "소요 블록 %s 블록"

func _ready():
	update_info()

func set_info(recipe: Dictionary):
	info = recipe

func update_info():
	# Setting names in UI
	var name = info.name
	name = name.left(name.length() -4)
	$panel/M/V/Title/Name.text = name_format % [name]
	
	# Getting exist
	for ing in info["ingredientIDList"]:
		var ing_ins = IngredientScn.instantiate() 
		ing_ins.set_ingredient_name(ing["name"])
		ingredients.add_child(ing_ins)
		
	for ing in info["foodIDList"]:
		var ing_ins = IngredientScn.instantiate() 
		ing_ins.set_ingredient_name(ing["name"])
		ingredients.add_child(ing_ins)	
		
	for tool in info["requiredKitchenEquipmentCategoryList"]:
		var tool_ins = IngredientScn.instantiate() 
		tool_ins.set_ingredient_name(tool["name"])
		tools.add_child(tool_ins)	

	set_block_req()
	
func set_block_req():
	req_block.text = block_format % [ info["requiredBlockCount"] ]
