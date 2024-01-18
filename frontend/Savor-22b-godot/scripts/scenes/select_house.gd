extends Control

const SELECT_HOUSE_BUTTON = preload("res://ui/house_slot_button.tscn")
const SLOT_IS_FULL = preload("res://ui/notice_popup.tscn")

@onready var noticepopup = $MarginContainer/Background/Noticepopup
@onready var gridcontainer = $MarginContainer/Background/HomeGridContainer

var houses = []

func _ready():
	print("select_house scene ready")
	var size = SceneContext.selected_village_capacity
	testinput()
	
	#var size = 12
	
	for i in range(size):
		var house = {"x" : size, "y" : 0, "owner" : "none"}
		houses.append(house)
		var button = SELECT_HOUSE_BUTTON.instantiate()
		button.set_house(house)
		button.button_down.connect(button_selected)
		gridcontainer.add_child(button)
	
	#for house in houses:
		#var button = SELECT_HOUSE_BUTTON.instantiate()
		#print(house)
		#button.set_house(house)
		#button.button_down.connect(button_selected)
		#
		#gridcontainer.add_child(button)
	
	pass

func testinput():
	houses.append({"x" : 12, "y" : 23, "owner" : "doge"})
	houses.append({"x" : 13, "y" : 55, "owner" : "hampter"})
	houses.append({"x" : 14, "y" : 78, "owner" : "kirby"})
	


func button_selected(house_index):
	var format_string = "house button down: %s"
	print(format_string % house_index)
	SceneContext.selected_house_index = house_index


func _on_button_pressed():
	get_tree().change_scene_to_file("res://scenes/select_village.tscn")


func _on_build_button_button_down():
	print_notice()

	
	
func print_notice():
	var box = SLOT_IS_FULL.instantiate()
	noticepopup.add_child(box)
