extends Control


func _ready():
	pass # Replace with function body.

func _input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_T:
			get_tree().change_scene_to_file("res://scenes/testpanel/test_panel.tscn")
			print("Test panel open")


func _on_button_pressed():
	get_tree().change_scene_to_file("res://scenes/farm.tscn")
