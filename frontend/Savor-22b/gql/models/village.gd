extends Resource

class_name VillageData

@export var id: int
@export var name: String
@export var width: int
@export var height: int
@export var worldX: int
@export var worldY: int
@export var houses: Array
@export var dungeons: Array

func from_dict(data: Dictionary) -> VillageData:
	var instance = VillageData.new()
	instance.id = data["id"]
	instance.name = data["name"]
	instance.width = data["width"]
	instance.height = data["height"]
	instance.worldX = data["worldX"]
	instance.worldY = data["worldY"]
	
	instance.houses = []
	for house in data["houses"]:
		instance.houses.append(HouseData.new().from_dict(house))
		
	instance.dungeons = []
	for dungeon in data["dungeons"]:
		instance.dungeons.append(DungeonData.new().from_dict(dungeon))
	return instance
