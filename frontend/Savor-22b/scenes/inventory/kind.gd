extends Node

var title: String
var enabled: bool

# Called when the node enters the scene tree for the first time.
func _ready():
	$"./Label".set_text(title)
	if enabled:
		$Label.label_settings.set_font_color(Color(255, 255, 255))
		$ColorRect.set_color(Color(0, 0, 0))
	else:
		$Label.label_settings.set_font_color(Color(0, 0, 0))
		$ColorRect.set_color(Color(255, 255, 255))

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
