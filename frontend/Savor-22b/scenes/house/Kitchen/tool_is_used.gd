extends Control

@onready var button = $Button

var data: Dictionary
var required_time
var menu_name
var state_id

var format_string = "[%s] 조리 중

남은 조리 블록 %s 블록
[%s]"

func _ready():
	update_text()

func update_text():
	if button == null:
		return
	
	var end_time = data["installedKitchenEquipment"]["cookingEndBlockIndex"]
	var current_time = SceneContext.block_index["blockQuery"]["blocks"][0]["index"]
	var time_left = end_time - current_time
	button.text = format_string % [menu_name, time_left, state_id]

func set_data(info: Dictionary):
	data = info
	menu_name = data["installedKitchenEquipment"]["cookingFood"]["name"]
	state_id = data["installedKitchenEquipment"]["cookingFood"]["stateId"]
	time_info()
	
func time_info():
	var recipe = SceneContext.recipe["recipe"]

	for single_recipe in recipe:
		var recipe_name = single_recipe["resultFood"]["name"]
		if(recipe_name == menu_name):
			required_time = single_recipe["requiredBlockCount"]
	
