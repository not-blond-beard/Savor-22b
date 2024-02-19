extends ColorRect

signal button_down(child_index: int)

@onready var button = $V/Button

var farm_slot: Dictionary

var format_string = "[%s] %s"

func _ready():
	_update_button()

func _update_button():
	if button == null:
		return

	button.text = format_string % [farm_slot.seedName, "수확하기"]


func set_farm_slot(farm_slot: Dictionary):
	self.farm_slot = farm_slot
	_update_button()

func _on_button_button_down():
	button_down.emit(get_index())
