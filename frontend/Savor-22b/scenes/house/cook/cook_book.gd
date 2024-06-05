extends Control

signal close_all
signal reload_signal
signal cook_started

const RecipeAvailableScn = preload("res://scenes/house/cook/recipe_available.tscn")

@onready var grid = $background/M/V/S/G
@onready var installed_tool = $background/M/V/Description/ToolList

var recipe_list = SceneContext.recipe["recipe"]
var installed_list = SceneContext.installed_tool_name
var available_tool_list : Array
var available_recipe_list : Array

var query_executor = QueryExecutor.new()
var create_food_query_executor
var stage_tx_mutation_executor

func _ready():
	create_food_query_executor = query_executor.create_food_query_executor
	stage_tx_mutation_executor = query_executor.stage_tx_mutation_executor
	add_child(create_food_query_executor)
	add_child(stage_tx_mutation_executor)

	var format_string = "[%s]"
	var unique_list = remove_duplicates(installed_list)
	installed_tool.text = format_string % [", ".join(unique_list)]
	
	filter_recipe()

	for recipe_data in recipe_list:
		for name in available_tool_list:
			for tool in recipe_data["requiredKitchenEquipmentCategoryList"]:
				if name == tool.name:
					available_recipe_list.append(recipe_data)
					var recipe = RecipeAvailableScn.instantiate()
					recipe.set_info(recipe_data)
					recipe.select_signal.connect(recipe_selected)
					grid.add_child(recipe)

func filter_recipe():
	# 작은 조리도구 이름 추출
	var small_tool_name : Array
	var all_equipment_list = SceneContext.shop["kitchenEquipments"]
	for equipment in all_equipment_list:
		if equipment["categoryType"] == "sub":
			small_tool_name.append(equipment["name"])
			
	available_tool_list.append_array(small_tool_name)
	available_tool_list.append_array(installed_list)
	
func remove_duplicates(array):
	var unique_elements = []
	for item in array:
		if not unique_elements.has(item):
			unique_elements.append(item)

	return unique_elements

func recipe_selected(recipe_index):
	SceneContext.selected_recipe_index = recipe_index + 1
	
	# Visual toggle
	for recipe in grid.get_children():
		if(recipe.get_index() != recipe_index):
			recipe.disable_button_selected()

func _on_close_button_down():
	queue_free()
	close_all.emit()

func _on_cook_button_down():
	var ingredient_arr = SceneContext.selected_ingredients
	var tool_arr = SceneContext.selected_tools
	var ingredient_str
	var tool_str
	
	if ingredient_arr.size() > 1 :
		ingredient_str = "\", \"".join(ingredient_arr)
	else:
		ingredient_str = ingredient_arr[0]
	if tool_arr.size() > 1 :
		tool_str = "\", \"".join(tool_arr)
	else:
		tool_str = tool_arr[0]
	
	query_executor.stage_action(
		{
			"publicKey": GlobalSigner.signer.GetPublicKey(),
			"recipeID": SceneContext.selected_recipe_index,
			"refrigeratorStateIdsToUse": ingredient_str,
			"kitchenEquipmentStateIdsToUse": tool_str
		},
		create_food_query_executor,
		stage_tx_mutation_executor
	)

	# Init past selected infos
	SceneContext.selected_ingredients = []
	SceneContext.selected_tools = []

	cook_started.emit()
