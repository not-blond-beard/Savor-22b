extends MarginContainer

signal button_down(child_index: int)

@onready var button = $Button

var village: Dictionary

func _ready():
	_update_button()

func _update_button():
	if button == null:
		return

	button.text = """%s
	(House %d/%d)""" % [
		village.name,
		village.houses.size(),
		int(village.width) * int(village.height)]

func set_village(village: Dictionary):
	self.village = village
	_update_button()

func _on_button_down():
	button_down.emit(get_index())
