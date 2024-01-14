extends Control

const SELECT_VILLAGE_VILLAGE_BUTTON = preload("res://ui/select_village_village_button.tscn")

@onready var villages_v_box_container = $LeftMarginContainer/VillagesVBoxContainer
@onready var village_info_label = $RightMarginContainer/MarginContainer/VBoxContainer/VillageInfoMarginContainer/ScrollContainer/Label

func _ready():
	print("select_village scene ready")
	for village in SceneContext.villages:
		var button = SELECT_VILLAGE_VILLAGE_BUTTON.instantiate()
		button.set_village(village)
		button.button_down.connect(_on_village_button_button_down)
		villages_v_box_container.add_child(button)
	
	_update_village_info_label()

func _update_village_info_label():
	var json_string = JSON.stringify(SceneContext.get_selected_village(), "\t")
	village_info_label.text = json_string

func _on_village_button_button_down(village_index):
	var format_string = "village button down: %s"
	print(format_string % village_index)
	SceneContext.selected_village_index = village_index
	_update_village_info_label()

func _on_start_button_button_down():
	print("start button down: %s" % SceneContext.selected_village_index)
	get_tree().change_scene_to_file("res://village_view/VillageView.tscn")
