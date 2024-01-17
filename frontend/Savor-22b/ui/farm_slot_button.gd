extends ColorRect

signal button_down(child_index: int)

@onready var button = $Button

var farm_slot: Dictionary

func _ready():
	_update_button()

func _update_button():
	if button == null:
		return

	button.text = """%s %s
	(%d %s)""" % [
		"벼",
		"자라는 중",
		1,
		"블록 남음"]

func set_farm_slot(farm_slot: Dictionary):
	self.farm_slot = farm_slot
	_update_button()

func _on_button_button_down():
	button_down.emit(get_index())
