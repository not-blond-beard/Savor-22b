extends Button

signal selected_var(state_id : String)

var format_string = "%s 등급 [%s개]"
var info

func _ready():
	pass

func set_ing_info(info : Array):
	self.info = info
	text = format_string % [info[0].grade, info.size()]

func _on_button_down():
	selected_var.emit(info[0].stateId)

