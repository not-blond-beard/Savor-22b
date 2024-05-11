extends Node

var villages: Array
var selected_village_index := 0
var user_state: Dictionary
var user_asset: String
var user_kitchen_state: Dictionary

var shop: Dictionary
var recipe: Dictionary

var selected_house_index := 0
var selected_house_location: Dictionary
var selected_village_capacity := 0
var selected_village_width := 0
var selected_village_height := 0

var selected_field_index := 0
var selected_item_index := 0
var selected_item_name : String

var selected_recipe_index := 1
var selected_ingredients : Array
var selected_tools : Array

var installed_tool_id : Array
var installed_tool_name : Array
var installed_tool_info : Array

var block_index : Dictionary

func set_villages(query_data: Dictionary):
	villages = query_data.data.villages

func set_user_state(query_data: Dictionary):
	user_state = query_data.data.userState

func set_user_kitchen_state(query_data: Dictionary):
	user_kitchen_state = query_data.data.userState

func set_user_asset(query_data: Dictionary):
	user_asset = query_data.data.asset

func get_selected_village():
	return villages[selected_village_index]

func set_shop(query_data: Dictionary):
	shop = query_data.data.shop
	
func set_recipe(query_data: Dictionary):
	recipe = query_data.data
	
func get_current_block(query_data: Dictionary):
	block_index = query_data.data
