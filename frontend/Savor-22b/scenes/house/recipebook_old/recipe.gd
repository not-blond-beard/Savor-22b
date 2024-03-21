extends Control

const ING_YES = preload("res://scenes/house/recipebook/ingredient_yes.tscn")
const ING_NO = preload("res://scenes/house/recipebook/ingredient_no.tscn")

@onready var ingredients = $panel/M/V/Description/Ingredients/list

var info
var inventorylist

var name_format = "[%s] 레시피"

func _ready():
	update_info()



func set_info(recipe: Dictionary):
	info = recipe

func update_info():
	inventorylist = SceneContext.user_state["inventoryState"]
	
	# Setting names in UI
	var name = info.name
	name = name.left(name.length() -4)
	$panel/M/V/Title/Name.text = name_format % [name]
	
	# Getting exist
	for ing in info["ingredientIDList"]:
		var created = false
		for inving in inventorylist.refrigeratorStateList:
			if ing["name"] == inving["name"]:
				var ing_y = ING_YES.instantiate()
				ing_y.set_ingname(ing["name"])
				ingredients.add_child(ing_y)
				created = true
				break
		
		if(!created):
			var ing_n = ING_NO.instantiate()
			ing_n.set_ingname(ing["name"])
			ingredients.add_child(ing_n)

	for ing in info["foodIDList"]:
		var created = false
		# refrigeratorstatelist에 만들어진 음식이 없는 경우 수정 필요
		for inving in inventorylist.refrigeratorStateList:
			if ing["name"] == inving["name"]:
				var ing_y = ING_YES.instantiate()
				ing_y.set_ingname(ing["name"])
				ingredients.add_child(ing_y)
				created = true
				break
		
		if(!created):
			var ing_n = ING_NO.instantiate()
			ing_n.set_ingname(ing["name"])
			ingredients.add_child(ing_n)
	
	
func insufficient():
	$panel/M/V/Title/insufficient.visible = true
