extends ColorRect

signal button_down

@onready var itemname = $M/V/Itemname
@onready var desc = $M/V/Description/Text

var item: Dictionary

var desc_format_string = "%s"

func _ready():
	update_item()


func update_item():
	if itemname == null:
		return
	
		# 아이템 설명이 추가되면 아이디 대신 이름 넣고 설명 란에 설명 추가하면 됩니다.
	itemname.text = str(item.id)
	desc.text = item.name

func set_item(info: Dictionary):
	item = info


func _on_buy_button_down():
	SceneContext.selected_item_index = item.id
	SceneContext.selected_item_name = item.name
	button_down.emit()
