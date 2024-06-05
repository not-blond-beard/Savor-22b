extends LineEdit

# Called when the node enters the scene tree for the first time.
func _ready():
	self.text = str(GlobalSigner.signer_address)

