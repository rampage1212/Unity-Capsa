using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneView : MonoBehaviour {
	public Image player1;
	public Image player2;
	public Image player3;
	public Image player4;

	SceneController controller;

	void Awake() {
		controller = GetComponent<SceneController> ();
	}

	public void OnMenuSceneLoaded() {
		player1.sprite = controller.avatars [0].normal;
		player2.sprite = controller.avatars [1].normal;
		player3.sprite = controller.avatars [2].normal;
		player4.sprite = controller.avatars [3].normal;
	}
	
	public void OnPlayer1Select() {
		controller.playerAvatarId = 0;
		controller.Play ();
	}

	public void OnPlayer2Select() {
		controller.playerAvatarId = 1;
		controller.Play ();
	}

	public void OnPlayer3Select() {
		controller.playerAvatarId = 2;
		controller.Play ();
	}

	public void OnPlayer4Select() {
		controller.playerAvatarId = 3;
		controller.Play ();
	}

	public void OnPlayAgain() {
		controller.Play ();
	}

	public void OnBackToMenu() {
		controller.Menu ();
	}
}
