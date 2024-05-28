extends ColorRect

signal button_down(x: int, y: int)

@onready var button = $Button

var entity = {}

var house_format_string = "%s의 집\n(x = %d, y = %d)"
var dungeon_format_string = "%s\n(x = %d, y = %d)"
var default_format_string = "(x = %d, y = %d)"

func _ready():
	pass
	
func _update_text():
	if entity.get("house", null) != null:
		button.text = house_format_string % [entity.house.owner.substr(0, 6), entity.x, entity.y]
	elif entity.get("dungeon", null) != null:
		button.text = dungeon_format_string % [entity.dungeon.name, entity.x, entity.y]
	else:
		button.text = default_format_string % [entity.x, entity.y]

func set_entity(entity):
	self.entity = entity
	_update_text()

func disable_button_selected():
	if(button.button_pressed):
		button.button_pressed = false

func disable_button():
	button.disabled = true

func _on_button_down():
	button_down.emit(entity.x, entity.y)
