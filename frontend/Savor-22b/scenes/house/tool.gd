extends Panel

@onready var toolname = $M/V/Name
@onready var tooldesc = $M/V/Desc

var info

func _ready():
	_update_info()


func _update_info():
	if toolname == null:
		return
	
	toolname.text = info.equipmentName
	tooldesc.text = info.stateId

func set_info(info: Dictionary):
	self.info = info
	
