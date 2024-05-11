extends Panel

@onready var tool_name = $M/V/Name
@onready var tool_description = $M/V/Desc
@onready var buy_button = $M/V/Buy

signal buy_signal

var info
var is_shop: bool = false

var desc_format_string = "%s : %s
%s : %s %s"

func _ready():
	_update_info()

func _update_info():
	if tool_name == null:
		return
	
	if (is_shop):
		tool_name.text = info.name
		tool_description.text = desc_format_string % [info.categoryType, info.categoryLabel, "Price", info.price, "BBG"]
		buy_button.visible = true
	else:
		tool_name.text = info.equipmentName
		tool_description.text = info.stateId

func set_info(info: Dictionary):
	self.info = info
	
func set_slot_type():
	self.is_shop = true
	_update_info()
	
func _on_buy_button_down():
	SceneContext.selected_item_index = info.id
	SceneContext.selected_item_name = info.name
	buy_signal.emit()
