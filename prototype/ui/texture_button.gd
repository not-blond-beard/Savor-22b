@tool
extends TextureButton

@export var label: String:
	set = set_label
	
@export var font_size: int = 30:
	set = set_font_size
@export var align_center: bool = true:
	set = set_align_center

func set_label(_label: String) -> void:
	label = _label
	
	$Label.text = label

func set_font_size(_font_size: int) -> void:
	font_size = _font_size

	$Label.add_theme_font_size_override("font_size", font_size)

func set_align_center(_align_center: bool) -> void:
	align_center = _align_center
	
	$Label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER if align_center else HORIZONTAL_ALIGNMENT_LEFT
