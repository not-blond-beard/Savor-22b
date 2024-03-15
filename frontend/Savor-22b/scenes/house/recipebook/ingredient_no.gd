extends Control

@onready var ingnametxt = $NoIng/Desc/Name
var ingname

func _ready():
	update_info()




func set_ingname(name: String):
	ingname = name

func update_info():
	ingnametxt.text = ingname
