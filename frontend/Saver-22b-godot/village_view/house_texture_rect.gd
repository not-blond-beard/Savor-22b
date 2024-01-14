extends TextureRect

signal button_down(position: Vector2)

func _on_button_button_down():
	print(position)
	button_down.emit(position)
