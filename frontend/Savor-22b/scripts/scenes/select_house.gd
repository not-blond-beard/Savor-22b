extends Control

const SELECT_HOUSE_BUTTON = preload("res://ui/house_slot_button.tscn")
const SLOT_IS_FULL = preload("res://ui/notice_popup.tscn")

@onready var noticepopup = $MarginContainer/Background/Noticepopup
@onready var gridcontainer = $MarginContainer/Background/MarginContainer/ScrollContainer/HomeGridContainer

var houses = []


func _ready():
	print("select_house scene ready")
	var size = SceneContext.selected_village_capacity
	testinput()

	
	gridcontainer.columns = SceneContext.selected_village_width
	
	var startxloc = -(( SceneContext.selected_village_width - 1 ) / 2)
	var startyloc = (SceneContext.selected_village_height -1 ) / 2
	var endxloc = ( SceneContext.selected_village_width - 1 ) / 2
	
	print("startxloc: %s" % startxloc)
	print("startyloc: %s" % startyloc)
	print("endxloc: %s" % endxloc)

	
	
	for i in range(size):
		var house = {"x" : startxloc, "y" : startyloc, "owner" : "none"}
		houses.append(house)
		var button = SELECT_HOUSE_BUTTON.instantiate()
		button.set_house(house)
		button.button_down.connect(button_selected)
		gridcontainer.add_child(button)		
		
		if(startxloc == endxloc):
			startyloc -= 1
			startxloc = -(( SceneContext.selected_village_width - 1 ) / 2)
		else:
			startxloc += 1


	
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
