extends Resource

class_name HouseData

@export var x: int
@export var y: int
@export var owner: String

func from_dict(data: Dictionary) -> HouseData:
	var instance = HouseData.new()
	instance.x = data["x"]
	instance.y = data["y"]
	instance.owner = data["owner"]
	return instance
