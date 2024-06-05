extends Control

signal install_tools

var data

func _ready():
	pass

func set_data(info : Dictionary):
	data = info

func _on_button_down():
	install_tools.emit(data.spaceNumber)
