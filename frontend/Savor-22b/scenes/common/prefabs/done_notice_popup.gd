extends ColorRect

@onready var info_text = $VBoxContainer/Label

var format_string = "%s %s"
var seed_name: String

func _ready():
	if info_text == null:
		return
		
	info_text.text = format_string % [seed_name, "(이)가 수확되었습니다."]

func set_seed_name(name: String):
	seed_name = name
	

func _on_button_down():
	queue_free()
