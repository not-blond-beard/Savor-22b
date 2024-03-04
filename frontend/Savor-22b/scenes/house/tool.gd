extends Panel

@onready var toolname = $M/V/Name
@onready var tooldesc = $M/V/Desc

var info
var isshop: bool = false

var desc_format_string = "%s : %s
%s : %s %s"

func _ready():
	_update_info()


func _update_info():
	if toolname == null:
		return
	
	if (isshop):
		toolname.text = info.name
		tooldesc.text = desc_format_string % [info.categoryType, info.categoryLabel, "Price", info.price, "BBG"]
	else:
		toolname.text = info.equipmentName
		tooldesc.text = info.stateId

func set_info(info: Dictionary):
	self.info = info
	
func set_slottype():
	self.isshop = true
	_update_info()
	
