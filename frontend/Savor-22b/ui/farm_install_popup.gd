extends Panel

signal accept_button_down

@onready var infotext = $VBoxContainer/MarginContainer/Text

var format_string = """%s 
	%s (%d %s)"""

var amount: int

func _ready():
	_update_text()


func _update_text():
	if infotext == null:
		return

	infotext.text = format_string % ["랜덤 종자 하나를 소모해서", "종자를 심기", amount, "개 남음"]
	
	

func set_amount(amount: int):
	self.amount = amount
	_update_text()

func _on_accept_button_down():
	accept_button_down.emit()


func _on_cancel_button_down():
	queue_free()
