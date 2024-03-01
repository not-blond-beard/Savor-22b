extends ColorRect

signal buy_button_down

func _ready():
	pass # Replace with function body.





func _on_buy_button_down():
	buy_button_down.emit()


func _on_cancel_button_down():
	queue_free()
