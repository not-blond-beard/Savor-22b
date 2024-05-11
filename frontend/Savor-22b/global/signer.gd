extends Node

var signer
var signer_address

func _ready():
	var script = load("res://scripts/sign/Signer.cs")
	if FileAccess.file_exists("user://privkey"):
		var f = FileAccess.open("user://privkey", FileAccess.READ)
		var content = f.get_as_text()
		signer = script.new(content)
		f.close()
	else:
		signer = script.Generate()
		var f = FileAccess.open("user://privkey", FileAccess.WRITE)
		f.store_string(signer.GetRaw())
		f.close()
	
	signer_address = signer.GetAddress()

func sign(unsignedTransaction: String) -> String:
	return self.signer.Sign(unsignedTransaction)
