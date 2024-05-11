extends ColorRect

signal refresh_me

func _ready():
	pass # Replace with function body.

func _on_button_down():
	refresh_me.emit()
	queue_free()
