extends ColorRect

signal button_down(child_index: int)

var is_left: bool

func _ready():
	pass

func _on_button_down():
	
	# return index of selected slot
	if (is_left):
		button_down.emit(get_index())
	else:
		button_down.emit(get_index()+5)

func im_right():
	is_left = false

func im_left():
	is_left = true
