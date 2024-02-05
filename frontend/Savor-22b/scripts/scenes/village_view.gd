extends Control

const SELECT_HOUSE_BUTTON = preload("res://ui/house_slot_button.tscn")

@onready var gridcontainer = $MarginContainer/Background/MarginContainer/ScrollContainer/HomeGridContainer

var houses = []
var existhouses = SceneContext.get_selected_village()["houses"]


func _ready():
	print("village view scene ready")
	var size = SceneContext.selected_village_capacity
	
	gridcontainer.columns = SceneContext.selected_village_width
	
	var startxloc = -(( SceneContext.selected_village_width - 1 ) / 2)
	var startyloc = (SceneContext.selected_village_height -1 ) / 2
	var endxloc = ( SceneContext.selected_village_width - 1 ) / 2
	
	print("startxloc: %s" % startxloc)
	print("startyloc: %s" % startyloc)
	print("endxloc: %s" % endxloc)

	
	#create blank slots
	for i in range(size):
		var house = {"x" : startxloc, "y" : startyloc, "owner" : "none"}
		houses.append(house)
		
		if(startxloc == endxloc):
			startyloc -= 1
			startxloc = -(( SceneContext.selected_village_width - 1 ) / 2)
		else:
			startxloc += 1

	for h1 in existhouses:
		for h2 in houses:
			if h1["x"] == h2["x"] and h1["y"] == h2["y"]:
				h2["owner"] = h1["owner"]
			
	for info in houses:
		var button = SELECT_HOUSE_BUTTON.instantiate()
		button.set_house(info)
		button.button_down.connect(button_selected)
		gridcontainer.add_child(button)
		
	disable_buttons()


func button_selected(house_index):
	var format_string1 = "house button down: %s"
	var format_string2 = "selected slot location: %s"
	print(format_string1 % house_index)
	print(format_string2 % houses[house_index])	
	SceneContext.selected_house_index = house_index
	SceneContext.selected_house_location = houses[house_index]

	#Toggle mode
	for slot in gridcontainer.get_children():
		if(slot.get_index() != house_index):
			slot.disable_button_selected()


func disable_buttons():
	print("button all disabled")
	for slot in gridcontainer.get_children():
		slot.disable_button()


func _on_home_button_button_down():
	get_tree().change_scene_to_file("res://scenes/select_village.tscn")



func _on_enter_button_button_down():
	pass # Replace with function body.

