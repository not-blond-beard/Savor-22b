extends ColorRect

signal button_down(child_index: int)
signal button_down_action
signal weed_action

@onready var button = $V/Button

var farm_slot: Dictionary

var format_string = """%s %s
	(%d %s)"""
	
var isleft: bool

func _ready():
	_update_button()

func _update_button():
	if button == null:
		return
		
	var currenttime = SceneContext.block_index["blockQuery"]["blocks"][0]["index"]
	var timeleft = (farm_slot.installedBlock + farm_slot.totalBlock) - currenttime
	button.text = format_string % [farm_slot.seedName, "자라는 중", timeleft, "블록 남음"]
	
	if (farm_slot.weedRemovalAble):
		pass

func set_farm_slot(farm_slot: Dictionary):
	self.farm_slot = farm_slot
	_update_button()

func _on_button_button_down():
	if (isleft):
		button_down.emit(get_index())
	else:
		button_down.emit(get_index()+5)
	button_down_action.emit()


func im_right():
	isleft = false

func im_left():
	isleft = true
