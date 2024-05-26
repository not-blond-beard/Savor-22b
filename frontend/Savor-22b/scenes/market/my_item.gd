extends Control

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
	pass # 물건 올리기 기능이 들어갈 예정
