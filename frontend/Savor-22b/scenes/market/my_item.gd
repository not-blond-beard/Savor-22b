extends Control

#@onready var food_name = $M/V/Name
#@onready var food_description = $M/V/Desc

@onready var item_button = $ItemSelectButton

var info
var desc_format_string = "%s
%s : %s
%s : %s"

func _ready():
	_update_info()

func _update_info():
	if item_button == null:
		return
	item_button.text = desc_format_string % [info.name, "등급", info.grade, "stateId", info.stateId]


func set_info(info: Dictionary):
	self.info = info
	

func _on_item_select_button_down():
	print(info)
