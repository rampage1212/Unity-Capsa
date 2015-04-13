using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrickView : MonoBehaviour {
	public GameObject resultPanel;

	TrickController controller;

	void Awake() {
		controller = GetComponent<TrickController> ();
	}

	public void NotifyMessage(string message) {
		Debug.Log (message);
	}

	public void OnDealSuccess(){
	}

	public void OnDealFailed(){
	}

	public void OnGameOver(){
		if (resultPanel == null)
			return;
		resultPanel.SetActive(true);

		resultPanel = resultPanel.transform.GetChild (0).gameObject;
		var p = resultPanel.transform.FindChild ("Player 1");
		p.transform.FindChild ("Avatar").GetComponent<Image> ().sprite = controller.players [0].View.Avatar.normal;
		p.transform.FindChild ("Card").GetComponent<Text> ().text = controller.players [0].Cards.Count.ToString ();
		p.transform.FindChild ("Points").GetComponent<Text> ().text = GetPoint (0).ToString();

		p = resultPanel.transform.FindChild ("Player 2");
		p.transform.FindChild ("Avatar").GetComponent<Image> ().sprite = controller.players [1].View.Avatar.normal;
		p.transform.FindChild ("Card").GetComponent<Text> ().text = controller.players [1].Cards.Count.ToString ();
		p.transform.FindChild ("Points").GetComponent<Text> ().text = GetPoint (1).ToString();

		p = resultPanel.transform.FindChild ("Player 3");
		p.transform.FindChild ("Avatar").GetComponent<Image> ().sprite = controller.players [2].View.Avatar.normal;
		p.transform.FindChild ("Card").GetComponent<Text> ().text = controller.players [2].Cards.Count.ToString ();
		p.transform.FindChild ("Points").GetComponent<Text> ().text = GetPoint (2).ToString();

		p = resultPanel.transform.FindChild ("Player 4");
		p.transform.FindChild ("Avatar").GetComponent<Image> ().sprite = controller.players [3].View.Avatar.normal;
		p.transform.FindChild ("Card").GetComponent<Text> ().text = controller.players [3].Cards.Count.ToString ();
		p.transform.FindChild ("Points").GetComponent<Text> ().text = GetPoint (3).ToString();

		resultPanel = resultPanel.transform.parent.gameObject;
	}

	int GetPoint(int playerId) {
		var cardLeft = controller.players [playerId].Cards.Count;
		if (cardLeft >= 13) {
			return cardLeft * -3;
		} else if (cardLeft >= 10) {
			return cardLeft * -2;
		} else if (cardLeft > 0) {
			return cardLeft * -1;
		} else {
			var point = 0;
			for (int i = 0; i < controller.players.Count; ++i) {
				if (i != playerId)
					point += GetPoint(i);
			}
			return Mathf.Abs(point);
		}
	}

	public void OnPlayAgain() {
		controller.PlayAgain ();
	}

	public void OnQuit() {
		controller.Quit ();
	}
}
