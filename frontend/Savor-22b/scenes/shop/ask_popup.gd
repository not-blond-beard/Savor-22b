extends ColorRect

signal buy_button_down

@onready var Itemname = $M/V/Itemname

var itemname
var format_string = "%s %s"

func _ready():
	update_info()



func update_info():
	if itemname == null:
		return
	
	Itemname.text = format_string % [itemname, "을(를)
	구매하시겠습니까?"]

func _on_buy_button_down():
	buy_button_down.emit()

func set_itemname(itemname: String):
	self.itemname = itemname
	
func _on_cancel_button_down():
	queue_free()
