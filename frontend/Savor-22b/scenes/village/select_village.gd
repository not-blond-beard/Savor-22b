extends Control

const SelectVillageButtonScn = preload("res://scenes/village/select_village_button.tscn")

@onready var villages_v_box_container = $LeftMarginContainer/VillagesVBoxContainer
@onready var village_info_label = $RightMarginContainer/MarginContainer/VBoxContainer/VillageInfoMarginContainer/ScrollContainer/Label

func _ready():
	Intro._query_villages()
	_update_village_info_label()
	for village in SceneContext.villages:
		var button = SelectVillageButtonScn.instantiate()
		button.set_village(village)
		button.button_down.connect(_on_village_button_down)
		villages_v_box_container.add_child(button)
	
	_update_village_info_label()

func _update_village_info_label():
	var json_string = JSON.stringify(SceneContext.get_selected_village(), "\t")
	village_info_label.text = json_string

func _on_village_button_down(village_index):
	var format_string = "village button down: %s"
	SceneContext.selected_village_index = village_index
	_update_village_info_label()

func _on_start_button_down():
	var village = SceneContext.get_selected_village()
	var capacity = village["height"] * village["width"]
	SceneContext.selected_village_capacity = capacity
	SceneContext.selected_village_width = village["width"]
	SceneContext.selected_village_height = village["height"]
		
	get_tree().change_scene_to_file("res://scenes/village/select_house.tscn")
