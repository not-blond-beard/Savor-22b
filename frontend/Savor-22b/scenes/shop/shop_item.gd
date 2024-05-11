extends ColorRect

signal button_down

@onready var item_name_label = $M/V/item_name
@onready var desc = $M/V/Description/Text

var item: Dictionary

var desc_format_string = "%s : %s %s"

func _ready():
	update_item()

func update_item():
	if item_name_label == null:
		return
	
	item_name_label.text = item.name
	desc.text = desc_format_string % ["가격", item.price, "BBG"]

func set_item(info: Dictionary):
	item = info

func _on_buy_button_down():
	SceneContext.selected_item_index = item.id
	SceneContext.selected_item_name = item.name
	button_down.emit()
