extends Control

@onready var item_name_label = $Background/MarginContainer/VBoxContainer/NameLabel
@onready var item_grade_label = $Background/MarginContainer/VBoxContainer/HBoxContainer/GradeLabel
@onready var item_amount_label = $Background/MarginContainer/VBoxContainer/HBoxContainer/AmountLabel

var info

func _ready():
	_update_info()

func _update_info():
	if $Background == null:
		return

	item_name_label.text = info[0]["name"]
	item_grade_label.text = info[0]["grade"] + "등급"
	item_amount_label.text = "%s 개" % [info.size()]

func set_info(info: Array):
	self.info = info

func _on_register_trade_button_down():
	pass # 물건 올리기 기능이 들어갈 예정
