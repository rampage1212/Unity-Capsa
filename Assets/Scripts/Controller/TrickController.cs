using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TrickView))]
public class TrickController : MonoBehaviour {
	public Card prefabs;
	public List<PokerHand> tricks;
	public List<PlayerController> players;

	TrickView view;
	int nextTurnPlayer;
	int lastTurnPlayer;
	int passPlayer;
	bool firstTurn = true;
	bool isGameOver = false;

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
			players[i].Cards = set.GetRange(i * 13, 13) as CardSet;
			
			// If have '3 diamond' card, play first turn
			var firstCard = players[i].Cards[0];
			if (firstCard.Nominal == "3" && firstCard.suit == Card.Suit.Diamond) {
				nextTurnPlayer = lastTurnPlayer = i;
			}
		}
		OnBeginTrick ();
	}

	void OnBeginTrick() {
		// Empty Cards Bet
		for (int i = 0; i < tricks.Count; ++i){
			for (int c = 0; c <tricks[i].Cards.Count; ++c){
				Destroy(tricks[i].Cards[c].gameObject);
			}
		}
		tricks.Clear ();
		
		// Reset all player state
		for (int i = 0; i < players.Count; ++i)
			players[i].OnTurnEnd ();
		
		passPlayer = 0;
		nextTurnPlayer = lastTurnPlayer;
		
		// Begin Turn
		NextTurn ();
	}

	void OnEndTrick() {
		OnBeginTrick ();
	}

	void NextTurn() {
		var current = nextTurnPlayer == 0 ? players.Count - 1 : nextTurnPlayer - 1;	
		players [current].OnTurnEnd ();
		
		while (players[nextTurnPlayer].Cards.Count <= 0) {
			nextTurnPlayer = nextTurnPlayer + 1 >= players.Count ? 0 : nextTurnPlayer + 1;
		}
		
		players [nextTurnPlayer].OnTurnBegin ();
		
		nextTurnPlayer = nextTurnPlayer + 1 >= players.Count ? 0 : nextTurnPlayer + 1;
	}

	public void Pass() {
		if (tricks.Count > 0) ++passPlayer;
		
		if (players.Count - passPlayer > 1)
			NextTurn ();
		else
			OnEndTrick ();
	}

	public bool Deal(PokerHand hand) {
		if (hand.Cards.Count == 0 || hand.Combination == PokerHand.CombinationType.Invalid) {
			view.NotifyMessage ("Invalid deal!");
			return false;
		}
		
		if (firstTurn && hand.Cards [0].Nominal != "3" && hand.Cards [0].suit != Card.Suit.Diamond) {
			view.NotifyMessage ("Must include 3 Diamond");
			return false;
		}
		firstTurn = false;
		
		// Initialize Tricks if still empty
		if (tricks.Count == 0) {
			tricks.Add (hand);
			lastTurnPlayer = nextTurnPlayer == 0 ? players.Count - 1 : nextTurnPlayer - 1;
			NextTurn ();
			return true;
		} else {
			var last = tricks[tricks.Count - 1];
			
			if (hand.Combination <= PokerHand.CombinationType.Triple && last.Combination <= PokerHand.CombinationType.Triple) {
				// Single, Pair or Triple rule
				if (hand.Combination == last.Combination && hand.Key > last.Key) {
					tricks.Add (hand);
					lastTurnPlayer = nextTurnPlayer == 0 ? players.Count - 1 : nextTurnPlayer - 1;
					NextTurn ();
					return true;
				}
			} else if (hand.Combination > PokerHand.CombinationType.Triple && last.Combination >  PokerHand.CombinationType.Triple) {
				// 5-card hand
				if (hand > last) {
					tricks.Add (hand);
					lastTurnPlayer = nextTurnPlayer == 0 ? players.Count - 1 : nextTurnPlayer - 1;
					NextTurn ();
					return true;
				}
			} else {
				view.NotifyMessage("Card Too Small or Combination is different");
			}
			return false;
		}
	}
}
