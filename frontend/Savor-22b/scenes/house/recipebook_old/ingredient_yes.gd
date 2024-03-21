extends Control

@onready var button = $Ing
var ingname
var format_string = "[%s]
보유 중"


func _ready():
	update_info()




func set_ingname(name: String):
	ingname = name

func update_info():
	button.text = format_string % [ingname]
