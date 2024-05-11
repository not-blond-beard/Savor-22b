extends Button

signal selected_var(state_id : String)

var format_string = "%s 등급 [%s개]"
var info

func _ready():
	pass

func set_tool_info(info : Array):
	self.info = info
	text = "%s [%s개]" % [info[0].equipmentName, info.size()]

func _on_button_down():
	selected_var.emit(info[0].stateId)
