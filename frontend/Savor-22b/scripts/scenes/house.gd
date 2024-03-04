extends Control

const HOUSE_INVENTORY = preload("res://scenes/house/house_inventory.tscn")

@onready var subscene = $M/V/subscene

func _ready():
	pass # Replace with function body.




func _on_inventory_button_button_down():
	var inventory = HOUSE_INVENTORY.instantiate()
	
	subscene.add_child(inventory)
