extends Control

#@onready var food_name = $M/V/Name
#@onready var food_description = $M/V/Desc

@onready var item_button = $ItemSelectButton
@onready var description_label = $BackgroundPanel/MarginContainer/VBoxContainer/DescriptionLabel

var info
var desc_format_string = "%s : %s
%s : %s%s
%s : %s"

func _ready():
	_update_info()

func _update_info():
	if description_label == null:
		return
	description_label.text = desc_format_string % ["품목명",info.food["name"],"가격",info.price," BBG","게시자",info.sellerAddress]


func set_info(info: Dictionary):
	self.info = info
	

func _on_item_select_button_down():
	print(info)
