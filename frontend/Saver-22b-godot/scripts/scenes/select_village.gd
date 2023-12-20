extends Control

var selected_village_index: int

func _ready():
	print("select_village scene ready")

func _on_village_button_button_down(extra_arg_0):
	var format_string = "village button down: %s"
	print(format_string % extra_arg_0)
	selected_village_index = extra_arg_0

func _on_start_button_button_down():
	print("start button down: %s" % selected_village_index)
	SceneContext.selected_village_index = selected_village_index
	get_tree().change_scene_to_file("res://village_view/VillageView.tscn")
