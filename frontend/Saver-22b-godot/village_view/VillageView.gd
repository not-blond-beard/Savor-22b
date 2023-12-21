class_name VillageViewClass
extends Node

var width: int
var height: int
var worldX: int
var worldY: int
var root_position: Vector2
var root_local_position: Vector2

@export_group("View nodes")
@export var bg: NinePatchRect
@export var root_rect: TextureRect

const Gql_query = preload("res://gql/query.gd")
const Origin_house = preload("res://village_view/house_texture_rect.tscn")
const Coordinate_weight = 300

# Called when the node enters the scene tree for the first time.
# 이건 Start의 성격일지 Awake의 성격일지 아직은 잘 모르겠음
func _ready():
	set_size()

# Mouse in viewport coordinates.
func _input(event):
	var mouse_event = event as InputEventMouseButton
	if mouse_event != null and mouse_event.is_released() and mouse_event.button_index == MOUSE_BUTTON_LEFT and mouse_event.is_command_or_control_pressed():
		print("Mouse Click/Unclick at: ", event.position)
		build_house()

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
	root_local_position = root_rect.global_position - bg.global_position

func instantiate_house(pos: Vector2):
	print("instantiate_house: ", pos)
	var house = Origin_house.instantiate()
	bg.add_child(house)
	house.set_size(Vector2(Coordinate_weight, Coordinate_weight))
	house.set_global_position(pos * Coordinate_weight + root_position - Vector2(Coordinate_weight / 2, Coordinate_weight / 2))

func build_house():
	var pos = bg.get_local_mouse_position()
	var relative_pos = pos - root_local_position
	relative_pos /= Coordinate_weight
	relative_pos.x = roundi(relative_pos.x)
	relative_pos.y = roundi(relative_pos.y)
	print("build house pos: ", relative_pos)
	print("root pos: ", root_local_position)
	print("mouse pos: ", pos)
	print("public key: ", GlobalSigner.signer.GetPublicKey())
	var gql_query = Gql_query.new()
	var query_string = gql_query.place_house_query_format.format([
			"\"%s\"" % GlobalSigner.signer.GetPublicKey(),
			SceneContext.get_selected_village()["id"],
			relative_pos.x,
			relative_pos.y], "{}")
	print(query_string)
	
	var query_executor = SvrGqlClient.raw(query_string)
	query_executor.graphql_response.connect(func(data):
		print("gql response: ", data)
		var unsigned_tx = data["data"]["createAction_PlaceUserHouse"]
		print("unsigned tx: ", unsigned_tx)
		var signature = GlobalSigner.sign(unsigned_tx)
		print("signed tx: ", signature)
		var mutation_executor = SvrGqlClient.raw_mutation(gql_query.stage_tx_query_format % [unsigned_tx, signature])
		mutation_executor.graphql_response.connect(func(data):
			print("mutation res: ", data)
		)
		add_child(mutation_executor)
		mutation_executor.run({})
	)
	add_child(query_executor)
	query_executor.run({})
