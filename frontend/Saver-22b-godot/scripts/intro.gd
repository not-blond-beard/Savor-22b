extends Control

func _ready():
	print("intro scene ready")

func _on_quit_button_button_down():
	print("quit button down")
	get_tree().quit()

func _on_play_button_button_down():
	print("play button down")
	get_tree().change_scene_to_file("res://scenes/select_village.tscn")
