extends Control

@onready var item_button = $ItemSelectButton
@onready var description_label = $BackgroundPanel/MarginContainer/VBoxContainer/DescriptionLabel

var info
var desc_format_string = "%s : %s
%s  %s
%s : %s
%s : %s%s"

func _ready():
	_update_info()

func _update_info():
	if description_label == null:
		return
	description_label.text = desc_format_string % ["품목명",info.food["name"],info.food["grade"]," 등급","stateId",info.food["stateId"],"가격",info.price," BBG"]

func set_info(info: Dictionary):
	self.info = info
