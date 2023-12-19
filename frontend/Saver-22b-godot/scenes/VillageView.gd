class_name VillageViewClass
extends Node

@export_group("Village model")
@export var width: int
@export var height: int
@export var worldX: int
@export var worldY: int

@export_group("View nodes")
@export var bg: NinePatchRect

# Called when the node enters the scene tree for the first time.
# 이건 Start의 성격일지 Awake의 성격일지 아직은 잘 모르겠음
func _ready():
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
# 이게 업데이트구나
func _process(delta):
	pass
	# var newView = VillageViewClass.new(1,2,3,4) 이런식으로 사용하려나

# 이게 생성자고
func _init(width:int, height: int, worldX: int, worldY: int):
	self.width = width
	self.height = height
	self.worldX = worldX
	self.worldY = worldY
