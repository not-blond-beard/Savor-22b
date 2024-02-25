extends ColorRect

@onready var infotext = $VBoxContainer/Label

var format_string = "%s %s"
var seedName: String

func _ready():
	if infotext == null:
		return
		

	infotext.text = format_string % [seedName, "(이)가 수확되었습니다."]

func set_seedname(name: String):
	seedName = name
	

func _on_button_button_down():
	queue_free()
