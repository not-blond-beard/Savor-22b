var stage_tx_mutation = GQLQuery.new("stageTransaction").set_args({
	"unsignedTransaction": "unsignedTransaction",
	"signature": "signature",
});

var get_villages_query = GQLQuery.new("villages").set_props([
	"id",
	"name",
	"width",
	"height",
	"worldX",
	"worldY"
]);

var calculate_relocation_cost_query = GQLQuery.new("calculateRelocationCost").set_args({
	"villageId": "villageId",
	"relocationVillageId": "relocationVillageId"
}).set_props([
	"durationBlocks",
	"price"
]);

var place_house_query = GQLQuery.new("createAction_PlaceUserHouse").set_args({
	"publicKey": "publicKey",
	"villageId": "villageId",
	"x": "x",
	"y": "y"
});

var plant_seed_query = GQLQuery.new("createAction_PlantingSeed").set_args({
	"publicKey": "publicKey",
	"fieldIndex": "fieldIndex",
	"itemStateIdToUse": "itemStateIdToUse"
});

var harvest_seed_query = GQLQuery.new("createAction_HarvestingSeed").set_args({
	"publicKey": "publicKey",
	"fieldIndex": "fieldIndex"
});

var remove_seed_query = GQLQuery.new("createAction_RemovePlantedSeed").set_args({
	"publicKey": "publicKey",
	"fieldIndex": "fieldIndex"
});

var remove_weed_query = GQLQuery.new("createAction_RemoveWeed").set_args({
	"publicKey": "publicKey",
	"fieldIndex": "fieldIndex"
});

var buy_shop_item_query = GQLQuery.new("createAction_BuyShopItem").set_args({
	"publicKey": "publicKey",
	"desiredShopItemID": "desiredShopItemID"
});

var buy_kitchen_equipment_query = GQLQuery.new("createAction_BuyKitchenEquipment").set_args({
	"publicKey": "publicKey",
	"desiredEquipmentID": "desiredEquipmentID"
});

var install_kitchen_equipment_query = GQLQuery.new("createAction_InstallKitchenEquipmentAction").set_args({
	"publicKey": "publicKey",
	"kitchenEquipmentStateID": "kitchenEquipmentStateID",
	"spaceNumber": "spaceNumber"
});

var create_food_query = GQLQuery.new("createAction_CreateFood").set_args({
	"publicKey": "publicKey",
	"recipeID": "recipeID",
	"refrigeratorStateIdsToUse": "refrigeratorStateIdsToUse",
	"kitchenEquipmentStateIdsToUse": "kitchenEquipmentStateIdsToUse"
});

var uninstall_kitchen_equipment_query = GQLQuery.new("createAction_UninstallKitchenEquipmentActionQuery").set_args({
	"publicKey": "publicKey",
	"spaceNumber": "spaceNumber"
});

var register_trade_good_query = GQLQuery.new("createAction_RegisterTradeGoodAction").set_args({
	"publicKey": "publicKey",
	"price": "price",
	"foodStateId": "foodStateId",
	"itemStateIds": "itemStateIds"
});

var trade_inventory_state_query = GQLQuery.new("tradeInventoryState").set_props([
	GQLQuery.new("tradeGoods").set_props([
		"sellerAddress",
		"productStateId",
		"price",
		"type",
		GQLQuery.new("food").set_props([
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
			"isAvailable"
		]),
		GQLQuery.new("items").set_props([
			"stateID",
			"itemID",
			"itemName"
		])
	])
])
