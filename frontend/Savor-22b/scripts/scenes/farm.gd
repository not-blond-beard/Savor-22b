extends Control

const FARM_SLOT_EMPTY = preload("res://ui/farm_slot_empty.tscn")
const FARM_SLOT_OCCUPIED = preload("res://ui/farm_slot_button.tscn")
const FARM_SLOT_DONE = preload("res://ui/farm_slot_done.tscn")

const Gql_query = preload("res://gql/query.gd")

@onready var leftfarm = $MC/HC/CR/MC/HC/Left
@onready var rightfarm = $MC/HC/CR/MC/HC/Right

var farms = []
var itemStateIdToUse

func _ready():
	print("farm scene ready")
	
	print(SceneContext.user_state["villageState"]["houseFieldStates"])
	farms = SceneContext.user_state["villageState"]["houseFieldStates"]
	
	#create blank slots
	for i in range(0,5):
		var farm
		if (farms[i] == null):
			farm = FARM_SLOT_EMPTY.instantiate()
			farm.im_left()
			farm.button_down.connect(farm_selected)
		else:
			if(farms[i]["isHarvested"]):
				farm = FARM_SLOT_DONE.instantiate()
				farm.set_farm_slot(farms[i])
			else:
				farm = FARM_SLOT_OCCUPIED.instantiate()
				farm.set_farm_slot(farms[i])
		
		leftfarm.add_child(farm)
		
	for i in range(5,10):
		var farm
		if (farms[i] == null):
			farm = FARM_SLOT_EMPTY.instantiate()
			farm.im_right()
			farm.button_down.connect(farm_selected)
		else:
			if(farms[i]["isHarvested"]):
				farm = FARM_SLOT_DONE.instantiate()
			else:
				farm = FARM_SLOT_OCCUPIED.instantiate()
		
		rightfarm.add_child(farm)	
	
	
func farm_selected(farm_index):
	var format_string = "farm selected: %s"
	print(format_string % farm_index)
	SceneContext.selected_field_index = farm_index

