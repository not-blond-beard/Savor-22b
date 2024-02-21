extends ColorRect

signal button_down(child_index: int)

var isleft: bool

func _ready():
	pass

func _on_button_button_down():
	
	if (isleft):
		button_down.emit(get_index())
	else:
		button_down.emit(get_index()+5)

func im_right():
	isleft = false

func im_left():
	isleft = true
