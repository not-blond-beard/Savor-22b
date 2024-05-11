extends Panel

@onready var foodname = $M/V/Name
@onready var fooddesc = $M/V/Desc


var info


var desc_format_string = "%s : %s
%s : %s %s"

func _ready():
	_update_info()


func _update_info():
	if foodname == null:
		return
	foodname.text = info.name
	fooddesc.text = info.stateId

func set_info(info: Dictionary):
	self.info = info
	


