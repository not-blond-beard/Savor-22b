extends Control

signal button_pressed

@onready var button = $Button

var data
var format_string = "%s
%s"

func _ready():
	update_text()

func update_text():
	if button == null:
		return

	button.text = format_string % [data.equipmentName, data.stateId]

func set_data(info: Dictionary):
	data = info
	

func _on_button_down():
	button_pressed.emit(data.stateId)
