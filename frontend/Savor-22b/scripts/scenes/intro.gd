extends Control

func _ready():
	print("intro scene ready")
	_all_ready()
	
func _all_ready():
	_query_villages()
	_query_user_state()
	_query_assets()
	_query_shop()
	_query_recipe()
	_query_kitchen_slot_state()
	
	get_current_block()
	print("block index")
	print(SceneContext.block_index)

func _on_quit_button_button_down():
	print("quit button down")
	get_tree().quit()

func _on_play_button_button_down():
	print("play button down")
	get_tree().change_scene_to_file("res://scenes/select_village.tscn")

func _query_villages():
	var query = GQLQuery.new("villages").set_props([
		"id",
		"name",
		"width",
		"height",
		"worldX",
		"worldY",
		GQLQuery.new("houses").set_props([
			"x",
			"y",
			"owner",
		]),
	])
	#print(query.serialize())
	var query_executor = SvrGqlClient.query('query', {}, query)
	query_executor.graphql_response.connect(
		func(data):
			SceneContext.set_villages(data)
	)
	add_child(query_executor)
	query_executor.run({})
	
func _query_assets():
	var query = GQLQuery.new("asset").set_args({
		"signer_address": "address",
	})
	#print(query.serialize())
	var query_executor = SvrGqlClient.query(
		'query',
		{
			"signer_address": "String!",
		},
		query)
	query_executor.graphql_response.connect(
		func(data):
			SceneContext.set_user_asset(data)
	)
	add_child(query_executor)
	query_executor.run({
		"signer_address": GlobalSigner.signer_address
	})

func _query_user_state():
	#print("signer address: %s" % GlobalSigner.signer_address)
	var query = GQLQuery.new("userState").set_args({
		"signer_address": "address",
	}).set_props([
		GQLQuery.new("inventoryState").set_props([
			GQLQuery.new("seedStateList").set_props([
				"stateId",
				"seedId",
				"name",
			]),
			GQLQuery.new("itemStateList").set_props([
				"stateID",
				"itemID",
				"itemName",
			]),
			GQLQuery.new("refrigeratorStateList").set_props([
				"stateId",
				"ingredientId",
				"foodID",
				"name",
				"grade",
				"hp",
				"attack",
				"defense",
				"speed",
				"isSuperFood",
				"isAvailable",
			]),
			GQLQuery.new("kitchenEquipmentStateList").set_props([
				"stateId",
				"equipmentId",
				"equipmentName",
				"blockTimeReductionPercent",
				"equipmentCategoryId",
				"equipmentCategoryName",
				"equipmentCategoryType",
				"isCooking",
				"cookingEndBlockIndex",
				GQLQuery.new("cookingFood").set_props([
					"stateId",
					"ingredientId",
					"foodID",
					"name",
					"grade",
					"hp",
					"attack",
					"defense",
					"speed",
					"isSuperFood",
					"isAvailable",
				]),
			]),
			GQLQuery.new("itemStateList").set_props([
				"stateID",
				"itemID",
				"itemName",
			]),
		]),
		GQLQuery.new("villageState").set_props([
			GQLQuery.new("houseState").set_props([
				"villageId",
			]),
			GQLQuery.new("houseFieldStates").set_props([
				"installedSeedGuid",
				"seedID",
				"installedBlock",
				"totalBlock",
				"lastWeedBlock",
				"weedRemovalCount",
				"seedName",
				"isHarvested",
				"weedRemovalAble"
			])
		])
	])
	#print(query.serialize())
	var query_executor = SvrGqlClient.query(
		'query',
		{
			"signer_address": "String!",
		},
		query)
	query_executor.graphql_response.connect(
		func(data):
			SceneContext.set_user_state(data)
	)
	add_child(query_executor)
	query_executor.run({
		"signer_address": GlobalSigner.signer_address
	})

func _query_shop():
	var query = GQLQuery.new("shop").set_props([
		GQLQuery.new("items").set_props([
			"id",
			"name",
			"price",
		]),
		GQLQuery.new("kitchenEquipments").set_props([
			"id",
			"categoryID",
			"categoryLabel",
			"categoryType",
			"name",
			"blockTimeReductionPercent",
			"price"
		])
	])
	#print(query.serialize())
	var query_executor = SvrGqlClient.query(
		'query',{}, query)
		
	query_executor.graphql_response.connect(
		func(data):
			SceneContext.set_shop(data)
	)
	add_child(query_executor)
	query_executor.run({})

func _query_recipe():
	var query = GQLQuery.new("recipe").set_props([
		"id",
		"name",
		"requiredBlockCount",
		GQLQuery.new("ingredientIDList").set_props([
			"id",
			"name",
		]),
		GQLQuery.new("foodIDList").set_props([
			"id",
			"name"
		]),
		GQLQuery.new("requiredKitchenEquipmentCategoryList").set_props([
			"id",
			"name",
		]),
		GQLQuery.new("resultFood").set_props([
			"id",
			"name",
		])
	])
	#print(query.serialize())
	var query_executor = SvrGqlClient.query(
		'query',{}, query)
		
	query_executor.graphql_response.connect(
		func(data):
			SceneContext.set_recipe(data)
	)
	add_child(query_executor)
	query_executor.run({})

func _query_kitchen_slot_state():
	#print("signer address: %s" % GlobalSigner.signer_address)
	
	var query = GQLQuery.new("userState").set_args({
		"signer_address": "address",
	}).set_props([
		GQLQuery.new("villageState").set_props([
			GQLQuery.new("houseState").set_props([
				GQLQuery.new("kitchenState").set_props([
					GQLQuery.new("firstApplianceSpace").set_props([
						"spaceNumber",
						GQLQuery.new("installedKitchenEquipment").set_props([
							"stateId",
							"equipmentId",
							"equipmentName",
							"blockTimeReductionPercent",
							"equipmentCategoryId",
							"equipmentCategoryName",
							"equipmentCategoryType",
							"isCooking",
							"cookingEndBlockIndex",
							GQLQuery.new("cookingFood").set_props([
								"stateId",
								"ingredientId",
								"foodID",
								"name",
								"grade",
								"hp",
								"attack",
								"defense",
								"speed",
								"isSuperFood",
								"level",
								"isAvailable",
							]),
						]),
					]),
					GQLQuery.new("secondApplianceSpace").set_props([
						"spaceNumber",
						GQLQuery.new("installedKitchenEquipment").set_props([
							"stateId",
							"equipmentId",
							"equipmentName",
							"blockTimeReductionPercent",
							"equipmentCategoryId",
							"equipmentCategoryName",
							"equipmentCategoryType",
							"isCooking",
							"cookingEndBlockIndex",
							GQLQuery.new("cookingFood").set_props([
								"stateId",
								"ingredientId",
								"foodID",
								"name",
								"grade",
								"hp",
								"attack",
								"defense",
								"speed",
								"isSuperFood",
								"level",
								"isAvailable",
							]),
						]),
					]),
					GQLQuery.new("thirdApplianceSpace").set_props([
						"spaceNumber",
						GQLQuery.new("installedKitchenEquipment").set_props([
							"stateId",
							"equipmentId",
							"equipmentName",
							"blockTimeReductionPercent",
							"equipmentCategoryId",
							"equipmentCategoryName",
							"equipmentCategoryType",
							"isCooking",
							"cookingEndBlockIndex",
							GQLQuery.new("cookingFood").set_props([
								"stateId",
								"ingredientId",
								"foodID",
								"name",
								"grade",
								"hp",
								"attack",
								"defense",
								"speed",
								"isSuperFood",
								"level",
								"isAvailable",
							]),
						]),
					])
				]),
			]),
		]),
	])
	
	#print(query.serialize())
	
	var query_executor = SvrGqlClient.query(
		'query',
		{
			"signer_address": "String!",
		},
		query
	)
	
	query_executor.graphql_response.connect(
		func(data):
			#print(data)
			SceneContext.set_user_kitchen_state(data)
	)
	
	add_child(query_executor)
	query_executor.run({
		"signer_address": GlobalSigner.signer_address
	})

func get_current_block():
	var query_string = "query
	{
		blockQuery
		{
			blocks(limit:1, desc:true)
			{
				hash
				index
			}
		}
	}"

	#print(query_string)
	
	var query_executor = SvrGqlClient2.raw(query_string)
	query_executor.graphql_response.connect(func(data):
		print("gql response: ", data)
		SceneContext.get_current_block(data)
	)
	add_child(query_executor)
	query_executor.run({})
