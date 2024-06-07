@tool
extends TextureButton

@export var label: String:
	set = set_label

func set_label(_label: String) -> void:
	label = _label
	
	$Label.text = label
