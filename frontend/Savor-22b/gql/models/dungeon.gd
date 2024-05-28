extends Resource

class_name DungeonData

@export var x: int
@export var y: int
@export var name: String

func from_dict(data: Dictionary) -> DungeonData:
	var instance = DungeonData.new()
	instance.x = data["x"]
	instance.y = data["y"]
	instance.name = data["name"]
	return instance
