extends Node

class_name Inventory

const KindScn = preload("res://scenes/inventory/kind.tscn")
const SlotScn = preload("res://scenes/inventory/slot.tscn")

# Input:
# Godot이 지원하지 않아서 주석으로 타입을 남깁니다.
# Dictionary[String, Array[SlotModel]]
var data: Dictionary

# State:
var current_kind: String

# Derived:
var kinds: Array
var current_slots: Array

# Called when the node enters the scene tree for the first time.
func _ready():
	assert (data.size() > 0)

	kinds = data.keys()
	current_kind = kinds[0]
	current_slots = data[current_kind]

	for kind in kinds:
		var kind_instance := KindScn.instantiate()
		kind_instance.title = kind
		kind_instance.enabled = kind == current_kind
		$"./ColorRect/VBoxContainer/Kinds".add_child(kind_instance)
	
	for slot in current_slots:
		var slot_instance := SlotScn.instantiate()
		slot_instance.title = slot.title
		slot_instance.icon = slot.icon
		slot_instance.count = slot.count
		$"./ColorRect/VBoxContainer/Slots".add_child(slot_instance)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
