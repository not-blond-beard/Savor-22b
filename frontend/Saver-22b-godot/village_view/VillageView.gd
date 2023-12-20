class_name VillageViewClass
extends Node

var width: int
var height: int
var worldX: int
var worldY: int
var root_position: Vector2

@export_group("View nodes")
@export var bg: NinePatchRect

# Called when the node enters the scene tree for the first time.
# 이건 Start의 성격일지 Awake의 성격일지 아직은 잘 모르겠음
func _ready():
	set_size()

# Called every frame. 'delta' is the elapsed time since the previous frame.
# 이게 업데이트구나
func _process(delta):
	pass
	# var newView = VillageViewClass.new(1,2,3,4) 이런식으로 사용하려나

func initialize(width: int, height: int, worldX: int, worldY: int, houses=[]):
	self.width = width
	self.height = height
	self.worldX = worldX
	self.worldY = worldY
	set_size()
	
	for house in houses.filter(func (argc): argc is Vector2):
		instantiate_house(house)

func set_size():
	bg.size.x = width
	bg.size.y = height
	root_position = Vector2(width/2, height/2)
	bg.set_position(root_position)

func instantiate_house(pos: Vector2):
	pass
