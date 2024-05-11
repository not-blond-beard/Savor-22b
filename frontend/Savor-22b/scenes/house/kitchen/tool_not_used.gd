extends Control

@onready var button = $Button

signal uninstall_big_tool_button_pressed

var data: Dictionary

var format_string = "[%s] 도구 설치됨

식재료 사용 시
요리 조리 가능"

func _ready():
	update_text()

func update_text():
	if button == null:
		return

	button.text = format_string % [data["installedKitchenEquipment"]["equipmentName"]]

func set_data(info: Dictionary):
	data = info
	
func _on_uninstall_button_pressed():
	uninstall_big_tool_button_pressed.emit(data.spaceNumber)
