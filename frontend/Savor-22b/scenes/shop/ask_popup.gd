extends ColorRect

signal buy_button_down

@onready var item_name_label = $M/V/item_name

var item_name
var format_string = "%s %s"

func _ready():
	update_info()

func update_info():
	if item_name == null:
		return
	
	item_name_label.text = format_string % [item_name, "을(를)
	구매하시겠습니까?"]

func _on_buy_button_down():
	buy_button_down.emit()

func set_item_name(item_name: String):
	self.item_name = item_name
	
func _on_cancel_button_down():
	queue_free()
