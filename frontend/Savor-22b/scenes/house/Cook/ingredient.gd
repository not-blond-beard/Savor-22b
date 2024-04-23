extends Control

signal select_ingredient(name : String)
signal select_tool(name : String)

@onready var button = $Ing
@onready var empty = $Empty

var ingname
var format_string = "[%s]
사용 가능"
var isTool = false

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

func change_state_text():
	button.text = "[%s]
	선택됨" % [ingname]

func button_is_tool():
	isTool = true

func _on_ing_button_down():
	if (isTool):
		select_tool.emit(ingname)
	else:
		select_ingredient.emit(ingname)
