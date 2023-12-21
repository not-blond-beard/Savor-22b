extends Control

func _ready():
	print("intro scene ready")
	_query_villages()
	_query_user_state()

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
	var query_executor = SvrGqlClient.query('query', {}, query)
	query_executor.graphql_response.connect(
		func(data):
			SceneContext.set_villages(data)
	)
	add_child(query_executor)
	query_executor.run({})

func _query_user_state():
	print("signer address: %s" % GlobalSigner.signer_address)
	var query = GQLQuery.new("userState").set_args({
		"signer_address": "address",
	}).set_props([
		GQLQuery.new("villageState").set_props([
			GQLQuery.new("houseFieldStates").set_props([
				"installedSeedGuid",
				"seedID",
				"installedBlock",
				"totalBlock",
				"lastWeedBlock",
				"weedRemovalCount",
				"seedName",
				"isHarvested",
				"weedRemovalAble",
			]),
			GQLQuery.new("houseState").set_props([
				"villageId",
				"positionX",
				"positionY",
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
								"isAvailable",
							]),
						]),
					]),
				]),
				"villageName",
			])
		]),
		GQLQuery.new("inventoryState").set_props([
			GQLQuery.new("seedStateList").set_props([
				"stateId",
				"seedId",
				"name",
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
	])
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
