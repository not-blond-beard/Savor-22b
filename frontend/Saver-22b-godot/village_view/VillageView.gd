class_name VillageViewClass
extends Node

var width: int
var height: int
var worldX: int
var worldY: int
var root_position: Vector2

@export_group("View nodes")
@export var bg: NinePatchRect

const Origin_house = preload("res://village_view/house_texture_rect.tscn")
const Coordinate_weight = 300

# Called when the node enters the scene tree for the first time.
# 이건 Start의 성격일지 Awake의 성격일지 아직은 잘 모르겠음
func _ready():
	set_size()

# Called every frame. 'delta' is the elapsed time since the previous frame.
# 이게 업데이트구나
func _process(delta):
	pass
	# var newView = VillageViewClass.new(1,2,3,4) 이런식으로 사용하려나

func initialize_by_village(village: Dictionary):
	initialize(
		village.width * Coordinate_weight,
		village.height * Coordinate_weight,
		village.worldX,
		village.worldY,
		village.houses.map(func(house): return Vector2(house.x, house.y))
	)

func initialize(width: int, height: int, worldX: int, worldY: int, houses=[]):
	self.width = width
	self.height = height
	self.worldX = worldX
	self.worldY = worldY
	set_size()

	for house in houses.filter(func (argc): return argc is Vector2):
		instantiate_house(house)

func set_size():
	bg.size.x = width
	bg.size.y = height
	bg.custom_minimum_size = bg.size
	bg.set_anchors_and_offsets_preset(Control.PRESET_CENTER, Control.PRESET_MODE_KEEP_SIZE)
	root_position = get_tree().root.size / 2 

func instantiate_house(pos: Vector2):
	print("instantiate_house: ", pos)
	var house = Origin_house.instantiate()
	bg.add_child(house)
	house.set_size(Vector2(Coordinate_weight, Coordinate_weight))
	house.set_global_position(pos * Coordinate_weight + root_position - Vector2(Coordinate_weight / 2, Coordinate_weight / 2))
