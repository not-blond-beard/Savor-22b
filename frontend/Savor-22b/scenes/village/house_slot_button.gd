extends ColorRect

signal button_down(child_index: int)

@onready var button = $Button

var house: Dictionary
var exist_houses = SceneContext.get_selected_village()["houses"]

var format_string = "%s 
(x = %d, y = %d)"

func _ready():
	_update_button()
	
func _update_button():
	if button == null:
		return
		
	button.text = format_string % [ house.owner, house.x, house.y ]

func set_house(house: Dictionary):
	self.house = house
	_update_button()
	
func update_owner():
	for h1 in exist_houses:
		if h1["x"] == house["x"] and h1["y"] == house["y"]:
			house["owner"] = h1["owner"]

func disable_button_selected():
	if(button.button_pressed):
		button.button_pressed = false
		
func disable_button():
	button.disabled = true

func _on_button_down():
	button_down.emit(get_index())
