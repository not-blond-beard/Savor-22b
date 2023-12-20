extends Node

var villages: Array
var selected_village_index := 0

func set_villages(query_data: Dictionary):
	villages = query_data.data.villages

func get_selected_village():
	return villages[selected_village_index]
