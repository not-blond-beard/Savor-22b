var get_villages_query = "query {
	villages {
		id
		name
		width
		height
		worldX
		worldY
	}
}"

var place_house_query_format = "query {
  createAction_PlaceUserHouse(
	publicKey: {},
  	villageId: {},
	x: {},
	y: {}
  )
}"

var stage_tx_query_format = "mutation {
	stageTransaction(
		unsignedTransaction:\"%s\",
		signature:\"%s\")
}"

var plant_seed_query_format = "query {
	createAction_PlantingSeed(
		publicKey: {},
		fieldIndex: {},
		itemStateIdToUse: {}
	)
}"


var harvest_seed_query_format = "query {
	createAction_HarvestingSeed(
		publicKey: {},
		fieldIndex: {}
	)
}"


var remove_seed_query_format = "query {
	createAction_RemovePlantedSeed(
		publicKey: {},
		fieldIndex: {}
	)
}"

var remove_weed_query_format = "query {
	createAction_RemoveWeed(
		publicKey: {},
		fieldIndex: {}
	)
}"



var buy_shop_item_query_format = "query {
	createAction_BuyShopItem(
		publicKey: {},
		desiredShopItemID: {}
	)
}"

var buy_kitchen_equipment_query_format = "query {
	createAction_BuyKitchenEquipment(
		publicKey: {},
		desiredEquipmentID: {}
	)
}"

var install_kitchen_equipment_query_format = "query {
	createAction_InstallKitchenEquipmentAction(
		publicKey: {},
		kitchenEquipmentStateID: {},
		spaceNumber: {}
	)
}"

var uninstall_kitchen_equipment_query_format = "query {
	createAction_UninstallKitchenEquipmentActionQuery(
		publicKey: {},
		spaceNumber: {},
	)
}"

var calculate_relocation_cost_query_template = GQLQuery.new("calculateRelocationCost").set_args({
	"villageId": "villageId",
	"relocationVillageId": "relocationVillageId",
}).set_props([
	"durationBlocks",
	"price",
])

var calculate_relocation_cost_query = SvrGqlClient.query(
	'CalculateRelocationCost',
	{
		"villageId": "Int!",
		"relocationVillageId": "Int!",
	},
	calculate_relocation_cost_query_template)
