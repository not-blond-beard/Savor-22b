extends Control

const FARM_SLOT_EMPTY = preload("res://ui/farm_slot_empty.tscn")
const FARM_SLOT_OCCUPIED = preload("res://ui/farm_slot_button.tscn")
const FARM_SLOT_DONE = preload("res://ui/farm_slot_done.tscn")

const INSTALL_POPUP = preload("res://ui/farm_install_popup.tscn")
const DONE_POPUP = preload("res://ui/done_notice_popup.tscn")

const Gql_query = preload("res://gql/query.gd")

@onready var leftfarm = $MC/HC/CR/MC/HC/Left
@onready var rightfarm = $MC/HC/CR/MC/HC/Right

@onready var popuparea = $Popups

var farms = []
var itemStateIds = []
var itemStateIdToUse

var harvestedName

var actionSuccess = false

func _ready():
	print("farm scene ready")
	
	print(SceneContext.user_state["villageState"]["houseFieldStates"])
	farms = SceneContext.user_state["villageState"]["houseFieldStates"]
	
	print(SceneContext.user_state["inventoryState"]["itemStateList"])
	itemStateIds = SceneContext.user_state["inventoryState"]["itemStateList"]
	
	#create blank slots
	for i in range(0,5):
		var farm
		if (farms[i] == null):
			farm = FARM_SLOT_EMPTY.instantiate()
			farm.im_left()
			farm.button_down.connect(farm_selected)
		else:
			if(farms[i]["isHarvested"]):
				farm = FARM_SLOT_DONE.instantiate()
				farm.im_left()
				farm.set_farm_slot(farms[i])
				farm.button_down.connect(farm_selected)
				farm.button_down_name.connect(harvested_name)
				farm.button_down_harvest.connect(harvest_seed)
			else:
				farm = FARM_SLOT_OCCUPIED.instantiate()
				farm.im_left()
				#insert farm.button_down.connect(farm_selected) if needed
				farm.set_farm_slot(farms[i])
		
		leftfarm.add_child(farm)
		
	for i in range(5,10):
		var farm
		if (farms[i] == null):
			farm = FARM_SLOT_EMPTY.instantiate()
			farm.im_right()
			farm.button_down.connect(farm_selected)
		else:
			if(farms[i]["isHarvested"]):
				farm = FARM_SLOT_DONE.instantiate()
				farm.im_right()
				farm.set_farm_slot(farms[i])
				farm.button_down.connect(farm_selected)
				farm.button_down_name.connect(harvested_name)
				farm.button_down_harvest.connect(harvest_seed)
			else:
				farm = FARM_SLOT_OCCUPIED.instantiate()
				farm.im_right()
				#insert farm.button_down.connect(farm_selected) if needed
				farm.set_farm_slot(farms[i])
		
		rightfarm.add_child(farm)
	
	
func farm_selected(farm_index):
	var format_string = "farm selected: %s"
	print(format_string % farm_index)
	SceneContext.selected_field_index = farm_index
	if (farms[farm_index] == null):
		plant_popup()
	#else:
		#if(farms[farm_index]["isHarvested"]):
			#done_popup()
		#else:
			#pass

func plant_popup():
	if is_instance_valid(popuparea):
		for child in popuparea.get_children():
			child.queue_free()
	
	var amount = itemStateIds.size()
	print(amount)
	var mousepos = get_local_mouse_position() + Vector2(0, -200)
	var installpopup = INSTALL_POPUP.instantiate()
	installpopup.set_amount(amount)
	popuparea.add_child(installpopup)
	installpopup.set_position(mousepos)
	installpopup.accept_button_down.connect(plant_seed)


func plant_seed():
	var gql_query = Gql_query.new()
	var query_string = gql_query.plant_seed_query_format.format([
		"\"%s\"" % GlobalSigner.signer.GetPublicKey(),
		SceneContext.selected_field_index,
		"\"%s\"" % itemStateIds[0]["stateID"]], "{}")
	print(query_string)
	
	var query_executor = SvrGqlClient.raw(query_string)
	query_executor.graphql_response.connect(func(data):
		print("gql response: ", data)
		var unsigned_tx = data["data"]["createAction_PlantingSeed"]
		print("unsigned tx: ", unsigned_tx)
		var signature = GlobalSigner.sign(unsigned_tx)
		print("signed tx: ", signature)
		var mutation_executor = SvrGqlClient.raw_mutation(gql_query.stage_tx_query_format % [unsigned_tx, signature])
		mutation_executor.graphql_response.connect(func(data):
			print("mutation res: ", data)
		)
		add_child(mutation_executor)
		mutation_executor.run({})
	)
	add_child(query_executor)
	query_executor.run({})

func harvested_name(seedName):
	harvestedName = seedName
	print(seedName)

func done_popup():
	print("알림 출력")
	if is_instance_valid(popuparea):
		for child in popuparea.get_children():
			child.queue_free()

	var donepopup = DONE_POPUP.instantiate()
	donepopup.set_seedname(harvestedName)
	popuparea.add_child(donepopup)
	# 팝업 위치는 설정이 필요할 듯
	donepopup.set_position(Vector2(700,500))


func harvest_seed():
	actionSuccess = false
	var gql_query = Gql_query.new()
	var query_string = gql_query.harvest_seed_query_format.format([
		"\"%s\"" % GlobalSigner.signer.GetPublicKey(),
		SceneContext.selected_field_index], "{}")
	print(query_string)
	
	var query_executor = SvrGqlClient.raw(query_string)
	query_executor.graphql_response.connect(func(data):
		print("gql response: ", data)
		var unsigned_tx = data["data"]["createAction_HarvestingSeed"]
		print("unsigned tx: ", unsigned_tx)
		var signature = GlobalSigner.sign(unsigned_tx)
		print("signed tx: ", signature)
		var mutation_executor = SvrGqlClient.raw_mutation(gql_query.stage_tx_query_format % [unsigned_tx, signature])
		mutation_executor.graphql_response.connect(func(data):
			print("mutation res: ", data)
		)
		add_child(mutation_executor)
		mutation_executor.run({})
	)
	add_child(query_executor)
	query_executor.run({})
	actionSuccess = true
	fetch_new()
	
func fetch_new():
# fetch datas
	Intro._query_user_state()
	
	# done popup
	if(actionSuccess):
		done_popup()
		actionSuccess = false
	
# delete old farm uis
	if is_instance_valid(leftfarm):
		for child in leftfarm.get_children():
			child.queue_free()
	if is_instance_valid(rightfarm):
		for child in rightfarm.get_children():
			child.queue_free()
	
	_ready()


func _on_refresh_button_button_down():
	fetch_new()
