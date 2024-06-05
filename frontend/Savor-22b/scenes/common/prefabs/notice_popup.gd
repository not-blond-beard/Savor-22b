extends ColorRect

func _on_button_pressed():
	queue_free()

func set_label(text):
	$MarginContainer/Label.text = text
