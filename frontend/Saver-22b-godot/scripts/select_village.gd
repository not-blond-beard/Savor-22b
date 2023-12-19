extends Control

var selected_village_index: int

func _ready():
	print("select_village scene ready")

func _on_village_button_button_down(extra_arg_0):
	var format_string = "village button down: %s"
	print(format_string % extra_arg_0)
	selected_village_index = extra_arg_0

func _on_start_button_button_down():
	var format_string = "start button down: %s"
	print(format_string % selected_village_index)
