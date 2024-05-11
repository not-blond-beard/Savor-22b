extends Control

signal select_signal(child_index: int)

const IngredientScn = preload("res://scenes/house/cook/ingredient.tscn")
const IngredientBigToolScn = preload("res://scenes/house/cook/ingredient_big_tool.tscn")
const SelectPopupScn = preload("res://scenes/house/cook/select_popup.tscn")

@onready var ingredients = $panel/M/V/Description/Ingredients/list
@onready var tools = $panel/M/V/Description/Tools
@onready var req_block = $panel/M/V/Blockreq
@onready var button = $panel
@onready var popup = $Popups

var info
var refrigerator = SceneContext.user_state["inventoryState"]["refrigeratorStateList"]
var toolbox = SceneContext.user_state["inventoryState"]["kitchenEquipmentStateList"]
var required_ingredients = []
var required_tools = []
var available_ingredients = []
var available_tools = []
var available_list = []
var name_format = "[%s] 레시피"
var block_format = "소요 블록 %s 블록"

func _ready():
	button.add_to_group("RecipeGroup")
	update_info()

func set_info(recipe: Dictionary):
	info = recipe

func update_info():
	# Setting names in UI
	var name = info.name
	name = name.left(name.length() -4)
	$panel/M/V/Title/Name.text = name_format % [name]
	
	# Getting exist ing/foods
	for ing in info["ingredientIDList"]:
		var ing_ins = IngredientScn.instantiate()
		ing_ins.name = ing["name"]
		ing_ins.set_ingredient_name(ing["name"])
		required_ingredients.append(ing["name"])
		ing_ins.select_ingredient.connect(select_ingredients)
		ingredients.add_child(ing_ins)
		
	for ing in info["foodIDList"]:
		var ing_ins = IngredientScn.instantiate() 
		ing_ins.name = ing["name"]
		ing_ins.set_ingredient_name(ing["name"])
		required_ingredients.append(ing["name"])
		ing_ins.select_ingredient.connect(select_ingredients)
		ingredients.add_child(ing_ins)
		
	# Getting exist tools
	for tool in info["requiredKitchenEquipmentCategoryList"]:
		if tool["name"] in SceneContext.installed_tool_name:
			var tool_ins = IngredientBigToolScn.instantiate()
			tool_ins.name = tool["name"]
			tool_ins.set_ingredient_name(tool["name"])
			required_tools.append(tool["name"])
			tool_ins.select_tool.connect(select_tools)
			tools.add_child(tool_ins)
		else:
			var tool_ins = IngredientScn.instantiate()
			tool_ins.name = tool["name"]
			tool_ins.set_ingredient_name(tool["name"])
			required_tools.append(tool["name"])
			tool_ins.button_is_tool()
			tool_ins.select_tool.connect(select_tools)
			tools.add_child(tool_ins)
		
	# Getting exist item infos // if not, disable button
	for ing in refrigerator:
		for req in required_ingredients:
			if (ing["name"] == req):
				available_ingredients.append(ing)
				available_list.append(req)
				
	for tool in toolbox:
		var full_tool_name = tool["equipmentName"]
		var tool_name = full_tool_name.split(" ")
		var tool_type
		if (full_tool_name.contains("고급")):
			tool_type = tool_name[1]
		else:
			tool_type = tool_name[0]

		for req in required_tools:
			if(tool_type == req):
				available_tools.append(tool)
				available_list.append(req)
				
	remove_duplicates(available_list)
	
	for ing in ingredients.get_children():
		var ingredient_name = ing.get_ingredient_name()
		if ingredient_name not in available_list:
			ing.disable_button()

	for tool in tools.get_children():
		var tool_name = tool.get_ingredient_name()
		if tool_name not in available_list:
			tool.disable_button()

	set_block_req()
	
func set_block_req():
	req_block.text = block_format % [ info["requiredBlockCount"] ]

func remove_duplicates(array):
	var unique_elements = []
	for item in array:
		if not unique_elements.has(item):
			unique_elements.append(item)

	return unique_elements

func disable_button_selected():
	if(button.button_pressed):
		button.button_pressed = false

func _on_panel_button_down():
	select_signal.emit(info.id)

func select_ingredients(name : String):
	var proper_ingredients = []
	for ing in available_ingredients:
		if ing["name"] == name:
			proper_ingredients.append(ing)
	
	var select_popup = SelectPopupScn.instantiate()
	popup.add_child(select_popup)
	select_popup.set_info(proper_ingredients)
	var mouse_pos = get_local_mouse_position() + Vector2(-200, -300)
	select_popup.set_position(mouse_pos)
	select_popup.disable_toggled.connect(disable_toggle)
	select_popup.enable_toggled.connect(enable_toggle)

func select_tools(name : String):
	var proper_tools = []
	for tool in available_tools:
		if tool["equipmentName"] == name:
			proper_tools.append(tool)
		if tool["equipmentName"] == "고급 " + name:
			proper_tools.append(tool)

	var select_popup = SelectPopupScn.instantiate()
	select_popup.set_is_tool()
	popup.add_child(select_popup)
	select_popup.set_info(proper_tools)
	var mouse_pos = get_local_mouse_position() + Vector2(-200, -100)
	select_popup.set_position(mouse_pos)
	select_popup.disable_toggled.connect(disable_toggle)
	select_popup.enable_toggled.connect(enable_toggle)

func disable_toggle(name: String):
	for ing in ingredients.get_children():
		if ing.name == name:
			ing.set_toggled(false)

func enable_toggle(name: String):
	for ing in ingredients.get_children():
		if ing.name == name:
			ing.set_toggled(true)
			ing.change_state_text()

func remove_select_popup():
	if popup != null:
		for pop in popup.get_children():
			pop.queue_free()
