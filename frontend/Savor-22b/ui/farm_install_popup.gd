extends Panel

signal accept_button_down

func _ready():
	pass # Replace with function body.



func _on_accept_button_down():
	accept_button_down.emit()


func _on_cancel_button_down():
	queue_free()
