using Godot;
using System;
using Libplanet.Crypto;

public partial class Signer : Node
{
	private readonly PrivateKey _privateKey;

	public Signer(string privateKeyHex)
		: this(PrivateKey.FromString(privateKeyHex))
	{
	}

	public Signer(PrivateKey privateKey)
	{
		_privateKey = privateKey;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Returns signature.
	public string Sign(string unsignedTransaction)
	{
		return Convert.ToHexString(_privateKey.Sign(Convert.FromHexString(unsignedTransaction)));
	}

	public string GetPublicKey()
	{
		return Convert.ToHexString(_privateKey.PublicKey.Format(false));
	}

	public string GetRaw()
	{
		return Convert.ToHexString(_privateKey.ToByteArray());
	}

	public string GetAddress()
	{
		return _privateKey.Address.ToString();
	}

	public static Signer Generate()
	{
		return new Signer(
			new PrivateKey()
		);
	}
}
