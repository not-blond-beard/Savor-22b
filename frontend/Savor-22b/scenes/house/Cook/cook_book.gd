extends Control

signal closeall
signal reload_signal
signal cook_started

const RECIPE = preload("res://scenes/house/Cook/recipe_available.tscn")

@onready var grid = $background/M/V/S/G
@onready var installed_tool = $background/M/V/Description/ToolList

const Gql_query = preload("res://gql/query.gd")

var recipe_list = SceneContext.recipe["recipe"]
var installed_list = SceneContext.installed_tool_name
var available_tool_list : Array
var available_recipe_list : Array

func _ready():
	var format_string = "[%s]"
	var unique_list = remove_duplicates(installed_list)
	installed_tool.text = format_string % [", ".join(unique_list)]
	
	filter_recipe()

	for recipe_data in recipe_list:
		print("recipe")
		print(recipe_data)
		for name in available_tool_list:
			for tool in recipe_data["requiredKitchenEquipmentCategoryList"]:
				if name == tool.name:
					available_recipe_list.append(recipe_data)
					var recipe = RECIPE.instantiate()
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
	SceneContext.selected_recipe_index = recipe_index
	
	# Visual toggle
	for recipe in grid.get_children():
		if(recipe.get_index() != recipe_index):
			recipe.disable_button_selected()


	


func _on_close_button_down():
	queue_free()
	closeall.emit()


func _on_cook_button_down():
	var ingarr = SceneContext.selected_ingredients
	var toolarr = SceneContext.selected_tools
	var ingstr
	var toolstr
	
	if ingarr.size() > 1 :
		ingstr = "\", \"".join(ingarr)
	else:
		ingstr = ingarr[0]
	if toolarr.size() > 1 :
		toolstr = "\", \"".join(toolarr)
	else:
		toolstr = toolarr[0]
	print(ingstr)
	print(toolstr)
	
	var gql_query = Gql_query.new()
	var query_string = gql_query.create_food_query_format.format([
		"\"%s\"" % GlobalSigner.signer.GetPublicKey(),
		SceneContext.selected_recipe_index,
		#need to be fixed
		"[\"%s\"]" % ingstr,
		"[\"%s\"]" % toolstr], "{}")
	print(query_string)
	
	var query_executor = SvrGqlClient.raw(query_string)
	query_executor.graphql_response.connect(func(data):
		print("gql response: ", data)
		var unsigned_tx = data["data"]["createAction_CreateFood"]
		print("unsigned tx: ", unsigned_tx)
		var signature = GlobalSigner.sign(unsigned_tx)
		print("signed tx: ", signature)
		var mutation_executor = SvrGqlClient.raw_mutation(gql_query.stage_tx_query_format % [unsigned_tx, signature])
		mutation_executor.graphql_response.connect(func(data):
			print("mutation res: ", data)
		)
		add_child(mutation_executor)
		mutation_executor.run({})
	)
	add_child(query_executor)
	query_executor.run({})

	# Init past selected infos
	SceneContext.selected_ingredients = []
	SceneContext.selected_tools = []

	cook_started.emit()
