extends ColorRect

signal button_down(child_index: int)
signal button_down_name(seed_name: String)
signal button_down_harvest

@onready var button = $V/Button

var farm_slot: Dictionary

var is_left: bool

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

func _on_button_down():
	button_down_name.emit(farm_slot.seedName)
	if (is_left):
		button_down.emit(get_index())
	else:
		button_down.emit(get_index()+5)

		button_down_harvest.emit()

func im_right():
	is_left = false

func im_left():
	is_left = true
