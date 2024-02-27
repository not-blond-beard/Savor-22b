extends ColorRect

signal button_down_remove


func _ready():
	pass # Replace with function body.


func _on_remove_button_down():
	button_down_remove.emit()

