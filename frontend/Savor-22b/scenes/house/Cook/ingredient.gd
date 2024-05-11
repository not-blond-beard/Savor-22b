extends Control

signal select_ingredient(name : String)
signal select_tool(name : String)

@onready var button = $Ing
@onready var empty = $Empty

var ingredient_name
var format_string = "[%s]
사용 가능"
var is_tool = false

func _ready():
	update_info()

func set_ingredient_name(name: String):
	ingredient_name = name

func get_ingredient_name():
	return ingredient_name
	
func disable_button():
	button.disabled = true
	empty.set_visible(true)
	button.text = "[%s]
	" % [ingredient_name]

func update_info():
	button.text = format_string % [ingredient_name]

func set_toggled(state: bool):
	button.set_toggle_mode(state)

func change_state_text():
	button.text = "[%s]
	선택됨" % [ingredient_name]

func button_is_tool():
	is_tool = true

func _on_ing_button_down():
	if (is_tool):
		select_tool.emit(ingredient_name)
	else:
		select_ingredient.emit(ingredient_name)
