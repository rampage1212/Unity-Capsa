using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteCollection
{
	public static Dictionary<string, SpriteCollection> instances = new Dictionary<string, SpriteCollection>();

	// Singleton pattern based function with multiple instances support
	public static SpriteCollection From(string spritesheet){
		if (!instances.ContainsKey (spritesheet)) {
			instances[spritesheet] = new SpriteCollection(spritesheet);
		}

		return instances [spritesheet];
	}

	private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

	private SpriteCollection(string spritesheet)
	{
		var data = Resources.LoadAll<Sprite>(spritesheet);

		for(var i = 0; i < data.Length; i++)
		{
			sprites[data[i].name] = data[i];
		}
	}
	
	public Sprite Get(string name)
	{
		return sprites[name];
	}
}