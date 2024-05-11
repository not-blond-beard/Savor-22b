extends Control

@onready var description = $P/Desc

var data
var required_time
var food_name

var format_string = "[%s] 요리 조리중
[%s 블록 남음]"

func _ready():
	update_label()

func update_label():
	if description == null:
		return
	
	description.text = format_string % [food_name, required_time]

func set_data(info: Dictionary):
	data = info
	
	# set end times
	var end_time = data["cookingEndBlockIndex"]
	var current_time = SceneContext.block_index["blockQuery"]["blocks"][0]["index"]
	required_time = end_time - current_time
	
	food_name = data["cookingFood"]["name"]
