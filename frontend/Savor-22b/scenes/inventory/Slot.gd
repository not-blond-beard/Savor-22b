extends Node

var title: String
var icon: String
var count: int

# Called when the node enters the scene tree for the first time.
func _ready():
	$Title.set_text("%s - %d" % [title, count])

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
