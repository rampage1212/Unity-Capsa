using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CardSet = System.Collections.Generic.List<Card>;

[RequireComponent(typeof(TrickView))]
public class TrickController : MonoBehaviour {
	public Card prefabs;
	public List<PlayerController> players;

	List<PokerHand> tricks = new List<PokerHand>();
	TrickView view;
	int nextTurnPlayer;
	int lastTurnPlayer;
	int passPlayer;
	bool firstTurn = true;

	// Singleton
	private static TrickController instance;
	public static TrickController Instance {
		get {
			if(instance == null)
				instance = GameObject.FindObjectOfType<TrickController>();
			return instance;
		}
	}

	public PokerHand LastTrick {
		get {
			return tricks.Count > 0 ? tricks[tricks.Count - 1] : null;
		}
	}

	public int CurrentPlayer {
		get {
			return nextTurnPlayer == 0 ? players.Count - 1 : nextTurnPlayer - 1;
		}
	}

	void Awake() {
		view = GetComponent<TrickView> ();
	}

	void Start() {
		// Initialize cards
		Card [] allCard = new Card[52];
		for (int i = 0; i < Card.nominalOrder.Length; ++i) {
			var spade = Instantiate (prefabs) as Card;
			spade.suit = Card.Suit.Spade;
			spade.Nominal = Card.nominalOrder [i];
			allCard [i * 4 + 0] = spade;
			
			var heart = Instantiate (prefabs) as Card;
			heart.suit = Card.Suit.Heart;
			heart.Nominal = Card.nominalOrder [i];
			allCard [i * 4 + 1] = heart;
			
			var club = Instantiate (prefabs) as Card;
			club.suit = Card.Suit.Club;
			club.Nominal = Card.nominalOrder [i];
			allCard [i * 4 + 2] = club;
			
			var diamond = Instantiate (prefabs) as Card;
			diamond.suit = Card.Suit.Diamond;
			diamond.Nominal = Card.nominalOrder [i];
			allCard [i * 4 + 3] = diamond;
		}
		new System.Random ().Shuffle (allCard);

		// Initialize players
		var set = new CardSet ();
		set.AddRange (allCard);
		for (int i = 0; i < players.Count; ++i) {
			players[i].id = i;
			players[i].Cards = set.GetRange(i * 13, 13);
			
			// If have '3 diamond' card, play first turn
			var firstCard = players[i].Cards[0];
			if (firstCard.Nominal == "3" && firstCard.suit == Card.Suit.Diamond) {
				nextTurnPlayer = lastTurnPlayer = i;
			}
		}
		OnBeginTrick ();
	}

	void OnBeginTrick() {		
		// Reset all player state
		for (int i = 0; i < players.Count; ++i)
			players[i].OnTurnEnd ();
		
		passPlayer = 0;
		nextTurnPlayer = lastTurnPlayer;
		
		// Begin Turn
		NextTurn ();
	}

	void OnEndTrick() {
		// Empty Cards
		for (int i = 0; i < tricks.Count; ++i){
			for (int c = 0; c < tricks[i].Cards.Count; ++c){
				Destroy(tricks[i].Cards[c].gameObject);
			}
		}
		tricks.Clear ();

		OnBeginTrick ();
	}

	void NextTurn() {
		var current = CurrentPlayer;	
		players [current].OnTurnEnd ();

		// Stop game if one player already spent all his cards
		Debug.Log (current + " => " + players [current].Cards.Count);
		if (players [current].Cards.Count == 0) {
			OnGameOver();
			return;
		}

		while (players[nextTurnPlayer].Cards.Count <= 0) {
			nextTurnPlayer = nextTurnPlayer + 1 >= players.Count ? 0 : nextTurnPlayer + 1;
		}
		
		players [nextTurnPlayer].OnTurnBegin ();
		
		nextTurnPlayer = nextTurnPlayer + 1 >= players.Count ? 0 : nextTurnPlayer + 1;
		firstTurn = false;
	}

	void OnGameOver() {
		Debug.Log ("GameOver");
		view.OnGameOver ();
	}

	public void Pass() {
		if (tricks.Count > 0) ++passPlayer;
		
		if (players.Count - passPlayer > 1)
			NextTurn ();
		else
			OnEndTrick ();
	}

	public bool Deal(PokerHand hand) {
		System.Action TakeCards = () => {
			for (int i = 0; i < hand.Cards.Count; ++i) {
				hand.Cards [i].transform.SetParent (TrickController.Instance.transform.GetChild (CurrentPlayer));
				hand.Cards [i].button.interactable = false;
				players [CurrentPlayer].Cards.Remove (hand.Cards [i]);
			}
		};

		if (hand.Cards.Count == 0 || hand.Combination == PokerHand.CombinationType.Invalid) {
			view.NotifyMessage ("Invalid deal!");
			return false;
		}
		
		if (firstTurn && hand.Cards [0].Nominal != "3" && hand.Cards [0].suit != Card.Suit.Diamond) {
			view.NotifyMessage ("Must include 3 Diamond");
			return false;
		}
		
		// Initialize Tricks if still empty
		if (tricks.Count == 0) {
			tricks.Add (hand);
			lastTurnPlayer = nextTurnPlayer == 0 ? players.Count - 1 : nextTurnPlayer - 1;
			TakeCards ();
			NextTurn ();
			return true;
		} else {
			var last = tricks[tricks.Count - 1];
			
			if (hand.Combination <= PokerHand.CombinationType.Triple && last.Combination <= PokerHand.CombinationType.Triple) {
				// Single, Pair or Triple rule
				if (hand.Combination == last.Combination && hand.Key > last.Key) {
					tricks.Add (hand);
					lastTurnPlayer = nextTurnPlayer == 0 ? players.Count - 1 : nextTurnPlayer - 1;
					TakeCards ();
					NextTurn ();
					return true;
				}
			} else if (hand.Combination > PokerHand.CombinationType.Triple && last.Combination >  PokerHand.CombinationType.Triple) {
				// 5-card hand
				if (hand > last) {
					tricks.Add (hand);
					lastTurnPlayer = nextTurnPlayer == 0 ? players.Count - 1 : nextTurnPlayer - 1;
					TakeCards ();
					NextTurn ();
					return true;
				}
			}
			view.NotifyMessage(hand.Combination + " " + hand.Key.Nominal + "!= or <" + LastTrick.Combination + " " + LastTrick.Key.Nominal);
			return false;
		}
	}
}
