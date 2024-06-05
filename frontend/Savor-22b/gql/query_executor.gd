extends Node

class_name QueryExecutor

const GqlQuery = preload("res://gql/query.gd");
var gql_query = GqlQuery.new();

var stage_tx_mutation_executor = SvrGqlClient.mutation(
	'StageTransaction',
	{
		"unsignedTransaction": "String!",
		"signature": "String!"
	},
	gql_query.stage_tx_mutation
);

var calculate_relocation_cost_query_executor = SvrGqlClient.query(
	'CalculateRelocationCost',
	{
		"villageId": "Int!",
		"relocationVillageId": "Int!"
	},
	gql_query.calculate_relocation_cost_query
);

var get_villages_query_executor = SvrGqlClient.query(
	'GetVillages',
	{},
	gql_query.get_villages_query
);

var place_house_query_executor = SvrGqlClient.query(
	'PlaceHouse',
	{
		"publicKey": "String!",
		"villageId": "Int!",
		"x": "Int!",
		"y": "Int!"
	},
	gql_query.place_house_query
);

var plant_seed_query_executor = SvrGqlClient.query(
	'PlantSeed',
	{
		"publicKey": "String!",
		"fieldIndex": "Int!",
		"itemStateIdToUse": "Guid!"
	},
	gql_query.plant_seed_query
);

var harvest_seed_query_executor = SvrGqlClient.query(
	'HarvestSeed',
	{
		"publicKey": "String!",
		"fieldIndex": "Int!"
	},
	gql_query.harvest_seed_query
);

var remove_seed_query_executor = SvrGqlClient.query(
	'RemoveSeed',
	{
		"publicKey": "String!",
		"fieldIndex": "Int!"
	},
	gql_query.remove_seed_query
);

var remove_weed_query_executor = SvrGqlClient.query(
	'RemoveWeed',
	{
		"publicKey": "String!",
		"fieldIndex": "Int!"
	},
	gql_query.remove_weed_query
);

var buy_shop_item_query_executor = SvrGqlClient.query(
	'BuyShopItem',
	{
		"publicKey": "String!",
		"desiredShopItemID": "Int!"
	},
	gql_query.buy_shop_item_query
);

var buy_kitchen_equipment_query_executor = SvrGqlClient.query(
	'BuyKitchenEquipment',
	{
		"publicKey": "String!",
		"desiredEquipmentID": "Int!"
	},
	gql_query.buy_kitchen_equipment_query
);

var install_kitchen_equipment_query_executor = SvrGqlClient.query(
	'InstallKitchenEquipment',
	{
		"publicKey": "String!",
		"kitchenEquipmentStateID": "Int!",
		"spaceNumber": "Int!"
	},
	gql_query.install_kitchen_equipment_query
);

var create_food_query_executor = SvrGqlClient.query(
	'CreateFood',
	{
		"publicKey": "String!",
		"recipeID": "Int!",
		"refrigeratorStateIdsToUse": "Int!",
		"kitchenEquipmentStateIdsToUse": "Int!"
	},
	gql_query.create_food_query
);

var uninstall_kitchen_equipment_query_executor = SvrGqlClient.query(
	'UninstallKitchenEquipment',
	{
		"publicKey": "String!",
		"spaceNumber": "Int!"
	},
	gql_query.uninstall_kitchen_equipment_query
);


var register_trade_good_query_executor = SvrGqlClient.query(
	'RegisterTradeGood',
	{
		"publicKey": "String!",
		"price": "Int!",
		"foodStateId": "Guid",
		"itemStateIds": "[Guid]"
	},
	gql_query.register_trade_good_query
);

var trade_inventory_state_executor = SvrGqlClient.query(
	'tradeInventoryState',
	{},
	gql_query.trade_inventory_state_query
);


func stage_action(params, query_executor, mutation_executor):
	query_executor.graphql_response.connect(
		func(data):
			var unsigned_tx = data["data"][data["data"].keys()[0]]
			var signature = GlobalSigner.sign(unsigned_tx)
			mutation_executor.run({
				"unsignedTransaction": unsigned_tx,
				"signature": signature
			})
	)
	query_executor.run(params)
