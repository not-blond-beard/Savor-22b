@tool
extends Control

@export var title: String:
	set = set_title
@export var label: String:
	set = set_label
@export var icon: Texture2D:
	set = set_icon

func set_title(_title: String) -> void:
	title = _title
	
	if $Container/Title:
		$Container/Title.text = title

func set_label(_label: String) -> void:
	label = _label
	
	if $Container/Value:
		$Container/Value.text = label

func set_icon(_icon: Texture2D) -> void:
	icon = _icon
	
	if $Container/Icon:
		$Container/Icon.texture = _icon
