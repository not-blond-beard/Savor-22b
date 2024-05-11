extends Control

signal select_tool(name : String)

@onready var button = $Ing
@onready var empty = $Empty

var ingname
var format_string = "[%s]
사용 가능"


func _ready():
	update_info()




func set_ingname(name: String):
	ingname = name



func get_ingname():
	return ingname
	
func disable_button():
	button.disabled = true
	empty.set_visible(true)
	button.text = "[%s]
	" % [ingname]

func update_info():
	button.text = format_string % [ingname]

func set_toggled(state: bool):
	button.set_toggle_mode(state)


func _on_ing_button_down():
	for tool in SceneContext.installed_tool_info:
		if tool.name == ingname && !tool.isCooking:
			SceneContext.selected_tools.append(tool.stateId)
