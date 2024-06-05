extends ColorRect

signal button_down_remove
signal weed_action_signal


func _ready():
	pass # Replace with function body.

func set_weed_button(weed : bool):
	if (weed):
		$M/V/Weed.visible = true

func _on_remove_button_down():
	button_down_remove.emit()

func _on_cancel_button_down():
	queue_free()


func _on_weed_button_down():
	weed_action_signal.emit()
	queue_free()
