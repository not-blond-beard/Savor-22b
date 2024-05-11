extends Control

signal disable_toggled
signal enable_toggled

const SelectButtonScn = preload("res://scenes/house/cook/select_button.tscn") 
const SelectToolButtonScn = preload("res://scenes/house/cook/select_tool_button.tscn")

@onready var panel = $Panel
@onready var ingredient_slot = $Panel/M/V/S/Ingredients
@onready var title = $Panel/M/V/Title

var ingredients
var ingredient_name
var ranks = []
var is_tool = false
var selected

func _ready():
	pass

func search_rank(info : Array):
	for ing in info:
		ranks.append(ing["grade"])

func rank_info(rank : String):
	var result = []
	for ing in ingredients:
		if ing["grade"] == rank:
			result.append(ing)
	return result
	
func create_button(info : Array):
	var rank_button = SelectButtonScn.instantiate()
	rank_button.selected_var.connect(set_selected)
	rank_button.set_ing_info(info)
	ingredient_slot.add_child(rank_button)
	
func create_tool_button(info : Array):
	var tool_button = SelectToolButtonScn.instantiate()
	tool_button.selected_var.connect(set_selected)
	tool_button.set_tool_info(info)
	ingredient_slot.add_child(tool_button)

func set_info(info : Array):
	if (!is_tool):
		ingredients = info
		ingredient_name = ingredients[0]["name"]
		search_rank(info)
		ranks = remove_duplicates(ranks)

		for rank in ranks:
			var rank_ing = rank_info(rank)
			create_button(rank_ing)
	else:
		ingredients = info
		var rares = []
		var normals = []
		for tool in ingredients:
			ingredient_name = ingredients[0]["equipmentName"]
			if tool["equipmentName"] == ingredient_name && !tool.isCooking:
				normals.append(tool)
			if tool["equipmentName"] == "고급 " + ingredient_name && !tool.isCooking:
				rares.append(tool)
		if (rares):
			create_tool_button(rares)
		if (normals):
			create_tool_button(normals)
		
	title.text = "현재 소유중인 [%s]" % [ingredient_name]

func remove_duplicates(array):
	var unique_elements = []
	for item in array:
		if not unique_elements.has(item):
			unique_elements.append(item)

	return unique_elements

func stay_selected():
	enable_toggled.emit(ingredient_name)
	queue_free()

func _on_panel_pressed():
	disable_toggled.emit(ingredient_name)
	queue_free()

func set_is_tool():
	is_tool = true

func set_selected(state_id : String):
	selected = state_id

func _on_button_down():
	if(is_tool):
		SceneContext.selected_tools.append(selected)
	else:
		SceneContext.selected_ingredients.append(selected)
	queue_free()
