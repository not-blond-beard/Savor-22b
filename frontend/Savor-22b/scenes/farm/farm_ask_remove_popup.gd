extends ColorRect

signal button_yes

func _ready():
	pass # Replace with function body.

func _on_accept_button_down():
	button_yes.emit()

func _on_cancel_button_down():
	queue_free()
