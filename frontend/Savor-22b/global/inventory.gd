extends Node

const inventory_scene = preload("res://scenes/inventory/inventory.tscn")

func open_inventory(data: Dictionary):
	var inventory_instance := inventory_scene.instantiate()
	inventory_instance.data = data

	get_tree().root.add_child(inventory_instance)
