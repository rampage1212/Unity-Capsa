using UnityEngine;
using UnityEngine.UI;

public class QuickBarView: MonoBehaviour {
	public Button straight;
	public Button flush;
	public Button fullHouse;
	public Button fourOfAKind;
	public Button straightFlush;
	public Button royalFlush;
	public Button dragon;

	void Awake () {
		straight = transform.FindChild ("Straight").GetComponent<Button>();
		flush = transform.FindChild ("Flush").GetComponent<Button>();
		fullHouse = transform.FindChild ("Full house").GetComponent<Button>();
		fourOfAKind = transform.FindChild ("4 of a kind").GetComponent<Button>();
		straightFlush = transform.FindChild ("Straight flush").GetComponent<Button>();
		royalFlush = transform.FindChild ("Royal flush").GetComponent<Button>();
		dragon = transform.FindChild ("Dragon").GetComponent<Button>();
	}

	public bool Interactable{
		set {
			straight.interactable = value;
			flush.interactable = value;
			fullHouse.interactable = value;
			fourOfAKind.interactable = value;
			straightFlush.interactable = value;
			royalFlush.interactable = value;
			dragon.interactable = value;
		}
	}
}
