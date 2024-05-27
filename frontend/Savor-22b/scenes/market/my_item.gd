extends Control

@onready var item_button = $ItemSelectButton
@onready var item_name_label = $ItemSelectButton/MarginContainer/VBoxContainer/HBoxContainer/NameLabel
@onready var item_grade_label = $ItemSelectButton/MarginContainer/VBoxContainer/HBoxContainer/GradeLabel
@onready var item_stateid_label = $ItemSelectButton/MarginContainer/VBoxContainer/StateIdLabel

var info

func _ready():
	_update_info()

func _update_info():
	if item_button == null:
		return

	item_name_label.text = info.name
	item_grade_label.text = info.grade + "등급"
	item_stateid_label.text = info.stateId

func set_info(info: Dictionary):
	self.info = info

func _on_item_select_button_down():
	pass # 물건 올리기 기능이 들어갈 예정
