extends ColorRect

signal button_down(child_index: int)

@onready var button = $Button

var house: Dictionary
var existhouses = SceneContext.get_selected_village()["houses"]

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
	for h1 in existhouses:
		if h1["x"] == house["x"] and h1["y"] == house["y"]:
			house["owner"] = h1["owner"]



func _on_button_button_down():
	button_down.emit(get_index())
