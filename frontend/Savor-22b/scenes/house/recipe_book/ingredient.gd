extends Control

@onready var button = $Ing
var ingredient_name
var format_string = "[%s]"

func _ready():
	update_info()

func set_ingredient_name(name: String):
	ingredient_name = name

func update_info():
	button.text = format_string % [ingredient_name]
