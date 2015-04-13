using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SceneView))]
public class SceneController : MonoBehaviour {
	public int playerAvatarId = 0;
	public PlayerView.AvatarSet [] avatars;
	public Sprite [] rawAvatars;

	SceneView view;

	// Singleton
	private static SceneController instance;
	public static SceneController Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<SceneController>();
				if (instance == null)
					Debug.LogException(new MissingReferenceException("Don't run capsa scene directly, run using menu scene"));
			}
			return instance;
		}
	}

	void Awake () {
		// Long life scene, never destroy it
		DontDestroyOnLoad (gameObject);
		view = GetComponent<SceneView> ();

		if (rawAvatars.Length % 3 != 0)
			Debug.LogError ("raw Avatars size is not enough");
		avatars = new PlayerView.AvatarSet[rawAvatars.Length / 3];
		int id = 0;
		for (int i = 0; i < rawAvatars.Length; i += 3) {
			avatars[id] = new PlayerView.AvatarSet();
			avatars[id].normal = rawAvatars[i + 0];
			avatars[id].happy = rawAvatars[i + 1];
			avatars[id].sad = rawAvatars[i + 2];
			++id;
		}
	}

	void Start() {
		view.OnMenuSceneLoaded ();
	}

	public void Play() {
		Application.LoadLevel (1);
	}

	public void Menu() {
		Destroy (gameObject);
		Application.LoadLevel (0);
	}
}
