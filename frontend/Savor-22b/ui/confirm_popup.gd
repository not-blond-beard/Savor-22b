extends ColorRect

signal ok_button_clicked_signal
signal cancel_button_clicked_signal

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func set_label(text):
	$VBoxContainer/Label.text = text

func _on_ok_button_pressed():
	ok_button_clicked_signal.emit()
	queue_free()


func _on_cancel_button_pressed():
	cancel_button_clicked_signal.emit()
	queue_free()
