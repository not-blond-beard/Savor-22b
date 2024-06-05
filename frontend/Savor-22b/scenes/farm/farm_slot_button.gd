extends ColorRect

signal button_down(child_index: int)
signal button_down_action

@onready var button = $V/Button

var farm_slot: Dictionary

var format_string = """%s %s
	(%d %s)"""
	
var is_left: bool

func _ready():
	_update_button()

func _update_button():
	if button == null:
		return
		
	var current_time = SceneContext.block_index["blockQuery"]["blocks"][0]["index"]
	var time_left = (farm_slot.installedBlock + farm_slot.totalBlock) - current_time
	button.text = format_string % [farm_slot.seedName, "자라는 중", time_left, "블록 남음"]
	
	if (farm_slot.weedRemovalAble):
		$Weed.visible = true

func set_farm_slot(farm_slot: Dictionary):
	self.farm_slot = farm_slot
	_update_button()

func _on_button_down():
	if (is_left):
		button_down.emit(get_index())
	else:
		button_down.emit(get_index()+5)
	button_down_action.emit(farm_slot.weedRemovalAble)


func im_right():
	is_left = false

func im_left():
	is_left = true
