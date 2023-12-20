using Godot;
using System;
using System.IO;
using Newtonsoft.Json.Linq;

public partial class Config : Node
{
    public string GraphQLUrl { get; private set; }

	public static Config Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
        LoadConfig();
	}

    public void LoadConfig()
    {
        string filePath = "res://config.json"; // 파일 경로
        string jsonText = File.ReadAllText(filePath);
        JObject config = JObject.Parse(jsonText);

        GraphQLUrl = config["GraphQLUrl"].ToString();
    }
}
