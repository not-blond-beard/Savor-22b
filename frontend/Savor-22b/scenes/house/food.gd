extends Panel

@onready var food_name = $M/V/Name
@onready var food_description = $M/V/Desc

var info
var desc_format_string = "%s : %s
%s : %s %s"

func _ready():
	_update_info()

func _update_info():
	if food_name == null:
		return
	food_name.text = info.name
	food_description.text = info.stateId

func set_info(info: Dictionary):
	self.info = info
	
