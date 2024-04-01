extends Control

@onready var Desc = $P/Desc

var data
var requiredtime
var foodname

var format_string = "[%s] 요리 조리중
[%s 블록 남음]"

func _ready():
	update_label()



func update_label():
	if Desc == null:
		return
	
	Desc.text = format_string % [foodname, requiredtime]

func set_data(info: Dictionary):
	data = info
	
	# set end times
	var endtime = data["cookingEndBlockIndex"]
	var currenttime = SceneContext.block_index["blockQuery"]["blocks"][0]["index"]
	requiredtime = endtime - currenttime
	
	foodname = data["cookingFood"]["name"]
