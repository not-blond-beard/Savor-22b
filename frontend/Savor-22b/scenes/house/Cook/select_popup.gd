extends Control

signal disable_toggled
signal enable_toggled

const BUTTON = preload("res://scenes/house/Cook/select_button.tscn") 
const TOOL_BUTTON = preload("res://scenes/house/Cook/select_tool_button.tscn")

@onready var panel = $Panel
@onready var ingredient_slot = $Panel/M/V/S/Ingredients
@onready var title = $Panel/M/V/Title

var ings
var ingname
var ranks = []
var isTool = false
var selected

func _ready():
	pass


func search_rank(info : Array):
	for ing in info:
		ranks.append(ing["grade"])

func rank_info(rank : String):
	var result = []
	for ing in ings:
		if ing["grade"] == rank:
			result.append(ing)
	return result
	
func create_button(info : Array):
	var rank_button = BUTTON.instantiate()
	rank_button.selected_var.connect(set_selected)
	rank_button.set_ing_info(info)
	ingredient_slot.add_child(rank_button)
	
func create_tool_button(info : Array):
	var tool_button = TOOL_BUTTON.instantiate()
	tool_button.selected_var.connect(set_selected)
	tool_button.set_tool_info(info)
	ingredient_slot.add_child(tool_button)

func set_info(info : Array):
	if (!isTool):
		ings = info
		ingname = ings[0]["name"]
		search_rank(info)
		ranks = remove_duplicates(ranks)

		for rank in ranks:
			var rank_ing = rank_info(rank)
			create_button(rank_ing)
	else:
		ings = info
		var rares = []
		var normals = []
		for tool in ings:
			ingname = ings[0]["equipmentName"]
			if tool["equipmentName"] == ingname && !tool.isCooking:
				normals.append(tool)
			if tool["equipmentName"] == "고급 " + ingname && !tool.isCooking:
				rares.append(tool)
		create_tool_button(rares)
		create_tool_button(normals)
		
	title.text = "현재 소유중인 [%s]" % [ingname]


func remove_duplicates(array):
	var unique_elements = []
	for item in array:
		if not unique_elements.has(item):
			unique_elements.append(item)

	return unique_elements

func stay_selected():
	enable_toggled.emit(ingname)
	queue_free()

func _on_panel_pressed():
	disable_toggled.emit(ingname)
	queue_free()

func is_tool():
	isTool = true

func set_selected(stateId : String):
	selected = stateId


func _on_button_button_down():
	if(isTool):
		SceneContext.selected_tools.append(selected)
	else:
		SceneContext.selected_ingredients.append(selected)
	queue_free()
