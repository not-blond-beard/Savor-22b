extends ColorRect

@onready var text = $Text

var asset
var format_string = "%s : %s"

func _ready():
	Intro._query_assets()
	asset = SceneContext.user_asset
	update_asset()
	

func update_asset():
	text.text = format_string % ["GOLD ", asset]

