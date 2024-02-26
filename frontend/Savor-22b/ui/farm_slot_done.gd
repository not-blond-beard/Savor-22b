extends ColorRect

signal button_down(child_index: int)
signal button_down_name(seedName: String)
signal button_down_harvest

@onready var button = $V/Button

var farm_slot: Dictionary

var isleft: bool

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
	button_down_name.emit(farm_slot.seedName)
	if (isleft):
		button_down.emit(get_index())
	else:
		button_down.emit(get_index()+5)

	print("done button signal")
	button_down_harvest.emit()

func im_right():
	isleft = false

func im_left():
	isleft = true
