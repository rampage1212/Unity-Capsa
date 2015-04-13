using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CardSet = System.Collections.Generic.List<Card>;

public class PlayerView : MonoBehaviour {
	[System.Serializable]
	public class AvatarSet {
		public Sprite normal;
		public Sprite happy;
		public Sprite sad;
	}

	public Image avatar_image;
	public GameObject panel;
	public GameObject passIndicator;
	public Text labelTimer;
	public Text labelCount;
	public QuickBarView quickbar;
	AvatarSet avatar;

	PlayerController controller;

	// Card Collection
	CardSet straight;
	CardSet flush;
	CardSet fullHouse;
	CardSet fourOfAKind;
	CardSet straightFlush;
	CardSet royalFlush;
	CardSet dragon;
	CardSet hint;

	PokerHand.CombinationType lastMarkCombination = PokerHand.CombinationType.Invalid;
	CardSet markedCards = new CardSet();

	void Awake(){
		controller = GetComponent<PlayerController> ();
	}

	// Helper Events for controller
	public void OnCardMarked(Card card) {
		markedCards.Add (card);
		lastMarkCombination = PokerHand.CombinationType.Invalid;
	}

	public void OnCardUnmarked(Card card) {
		markedCards.Remove (card);
		lastMarkCombination = PokerHand.CombinationType.Invalid;
	}

	public void OnTurnBegin() {
		if (panel)
			panel.SetActive (true);
		if (passIndicator)
			passIndicator.SetActive (false);
		avatar_image.sprite = avatar.normal;
	}
	
	public void OnTurnEnd() {
		if (panel)
			panel.SetActive (false);
		if (quickbar)
			quickbar.Interactable = false;
		if (passIndicator)
			passIndicator.SetActive (false);
	}

	public void OnDeal() {
		controller.Deal (markedCards);
	}

	public void OnDealSuccess() {
		markedCards.Clear ();
		this.TotalCard = controller.Cards.Count;
		avatar_image.sprite = avatar.happy;
	}

	public void OnDealFailed() {
	}

	public void OnPass() {
		passIndicator.SetActive (controller.IsPass);
		avatar_image.sprite = avatar.sad;
	}

	// Helper events for interface
	public void OnSelectStraight() {
		MarkAll (straight, lastMarkCombination != PokerHand.CombinationType.Straight);
		lastMarkCombination = PokerHand.CombinationType.Straight;
	}
	
	public void OnSelectFlush() {
		MarkAll (flush, lastMarkCombination != PokerHand.CombinationType.Flush);
		lastMarkCombination = PokerHand.CombinationType.Flush;
	}
	
	public void OnSelectFullHouse() {
		MarkAll (fullHouse, lastMarkCombination != PokerHand.CombinationType.FullHouse);
		lastMarkCombination = PokerHand.CombinationType.FullHouse;
	}
	
	public void OnSelectFourOfAKind() {
		MarkAll (fourOfAKind, lastMarkCombination != PokerHand.CombinationType.FourOfAKind);
		lastMarkCombination = PokerHand.CombinationType.FourOfAKind;
	}
	
	public void OnSelectStraightFlush() {
		MarkAll (straightFlush, lastMarkCombination != PokerHand.CombinationType.StraightFlush);
		lastMarkCombination = PokerHand.CombinationType.StraightFlush;
	}
	
	public void OnSelectRoyalFlush() {
		MarkAll (royalFlush, lastMarkCombination != PokerHand.CombinationType.RoyalFlush);
		lastMarkCombination = PokerHand.CombinationType.RoyalFlush;
	}
	
	public void OnSelectDragon() {
		MarkAll (dragon, lastMarkCombination != PokerHand.CombinationType.Dragon);
		lastMarkCombination = PokerHand.CombinationType.Dragon;
	}
	
	public void OnSelectHint() {
		MarkAll (hint, true);
		lastMarkCombination = PokerHand.CombinationType.Invalid;
	}

	void MarkAll(CardSet set, bool reset) {
		if (set == null)
			return;

		// Deselect all
		if (reset) {
			for (int i = 0; i < controller.Cards.Count; ++i) {
				controller.Cards [i].Select (false);
			}
		}
		
		// Select new card
		for (int i = 0; i < set.Count; ++i) {
			set[i].ToggleSelect();
		}
	}

	public void Display (CardSet set) {
		for (int i = 0; i < set.Count; ++i){
			set[i].transform.SetParent(transform, false);
		}
	}

	// Property for Card Collection
	public CardSet MarkedCards {
		get { return markedCards; }
	}

	public int TotalCard {
		set { if (labelCount) labelCount.text = "" + value; }
	}

	public float TimeLeft {
		set { if (labelTimer) labelTimer.text = "" + Mathf.CeilToInt(value); }
	}

	public CardSet Straight {
		get { return straight; }
		set {
			straight = value;
			if (quickbar) quickbar.straight.interactable = true;
		}
	}
	
	public CardSet Flush {
		get { return flush; }
		set {
			flush = value;
			if (quickbar) quickbar.flush.interactable = true;
		}
	}
	
	public CardSet FullHouse {
		get { return fullHouse; }
		set {
			fullHouse = value;
			if (quickbar) quickbar.fullHouse.interactable = true;
		}
	}

	public CardSet FourOfAKind {
		get { return flush; }
		set {
			fourOfAKind = value;
			if (quickbar) quickbar.fourOfAKind.interactable = true;
		}
	}
	
	public CardSet StraightFlush {
		get { return straightFlush; }
		set {
			straightFlush = value;
			if (quickbar) quickbar.straightFlush.interactable = true;
		}
	}

	public CardSet RoyalFlush {
		get { return royalFlush; }
		set {
			royalFlush = value;
			if (quickbar) quickbar.royalFlush.interactable = true;
		}
	}
	
	public CardSet Dragon {
		get { return dragon; }
		set {
			dragon = value;
			if (quickbar) quickbar.dragon.interactable = true;
		}
	}

	public CardSet Hint {
		get { return hint; }
		set { hint = value; }
	}

	public AvatarSet Avatar {
		get { return avatar; }
		set { 
			avatar = value;
			avatar_image.sprite = avatar.normal;
		}
	}
}