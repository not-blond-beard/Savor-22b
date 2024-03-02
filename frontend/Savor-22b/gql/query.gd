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


var buy_shop_item_query_format = "query {
	createAction_BuyShopItem(
		publicKey: {},
		desiredShopItemID: {}
	)
}"
