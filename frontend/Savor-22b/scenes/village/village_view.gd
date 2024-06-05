extends Control

const HouseSlotButtonScn = preload("res://scenes/village/house_slot_button.tscn")
const SystemShopScn = preload("res://scenes/shop/system_shop.tscn")

@onready var grid_container = $MarginContainer/Background/MarginContainer/ScrollContainer/HomeGridContainer
@onready var popups = $Popups

var houses = []
var exist_houses = SceneContext.get_selected_village()["houses"]

func _ready():
	var size = SceneContext.selected_village_capacity
	
	grid_container.columns = SceneContext.selected_village_width
	
	var start_x_loc = -(( SceneContext.selected_village_width - 1 ) / 2)
	var start_y_loc = (SceneContext.selected_village_height -1 ) / 2
	var end_x_loc = ( SceneContext.selected_village_width - 1 ) / 2
	
	#create blank slots
	for i in range(size):
		var house = {"x" : start_x_loc, "y" : start_y_loc, "owner" : "none"}
		houses.append(house)
		
		if(start_x_loc == end_x_loc):
			start_y_loc -= 1
			start_x_loc = -(( SceneContext.selected_village_width - 1 ) / 2)
		else:
			start_x_loc += 1

	for h1 in exist_houses:
		for h2 in houses:
			if h1["x"] == h2["x"] and h1["y"] == h2["y"]:
				h2["owner"] = h1["owner"]
			
	for info in houses:
		var button = HouseSlotButtonScn.instantiate()
		button.set_house(info)
		button.button_down.connect(button_selected)
		grid_container.add_child(button)
		
	disable_buttons()

func button_selected(house_index):
	var format_string1 = "house button down: %s"
	var format_string2 = "selected slot location: %s"
	SceneContext.selected_house_index = house_index
	SceneContext.selected_house_location = houses[house_index]

	#Toggle mode
	for slot in grid_container.get_children():
		if(slot.get_index() != house_index):
			slot.disable_button_selected()

func disable_buttons():
	for slot in grid_container.get_children():
		slot.disable_button()

func _on_home_button_down():
	get_tree().change_scene_to_file("res://scenes/village/select_village.tscn")

func _on_enter_button_down():
	pass # Replace with function body.

func _on_farm_button_down():
	get_tree().change_scene_to_file("res://scenes/farm/farm.tscn")



#open shop with S input
func _input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_S:
			var shop = SystemShopScn.instantiate()
			popups.add_child(shop)
			shop.set_position(Vector2(400, 150))


