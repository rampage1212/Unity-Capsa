using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BigTwo : MonoBehaviour {
	public enum Combination {
		Invalid,
		Single,
		Pairs,
		Triples,
		Straight,
		Flush,
		FullHouse,
		FourOfAKind,
		StraightFlush,
		RoyalFlush,
		Dragon
	}

	public struct Bet {
		public Card key;
		public Card [] cards;
		public Combination combination;
	}

	public Card cardPrefab;
	public List<Bet> bets = new List<Bet> ();
	public Text commentLabel;

	public List<DeckController> players = new List<DeckController>();
	public List<DeckController> finishedPlayers = new List<DeckController>();

	int nextTurn;
	int lastBetPlayer;
	int skippingPlayer;
	bool firstTurn = true;
	bool isGameOver = false;
	
	void Start() {
		// Initialize cards
		Card [] allCard = new Card[52];
		for (int i = 0; i < Card.nominalOrder.Length; ++i) {
			var spade = Instantiate(cardPrefab) as Card;
			spade.suit = Card.Suit.Spade;
			spade.Nominal = Card.nominalOrder[i];
			allCard[i * 4 + 0] = spade;

			var heart = Instantiate(cardPrefab) as Card;
			heart.suit = Card.Suit.Heart;
			heart.Nominal = Card.nominalOrder[i];
			allCard[i * 4 + 1] = heart;

			var club = Instantiate(cardPrefab) as Card;
			club.suit = Card.Suit.Club;
			club.Nominal = Card.nominalOrder[i];
			allCard[i * 4 + 2] = club;

			var diamond = Instantiate(cardPrefab) as Card;
			diamond.suit = Card.Suit.Diamond;
			diamond.Nominal = Card.nominalOrder[i];
			allCard[i * 4 + 3] = diamond;
		}
		new System.Random ().Shuffle (allCard);


		// Initialize players
		var cardList = allCard.ToList();
		for (int i = 0; i < players.Count; ++i) {
			players[i].bigTwo = this;
			players[i].playerId = i;
			players[i].Cards = new List<Card>(cardList.GetRange(i * 13, 13));

			// If have '3 diamond' card, play first turn
			var firstCard = players[i].Cards[0];
			Debug.Log(firstCard.Nominal + " " + firstCard.suit);
			if (firstCard.Nominal == "3" && firstCard.suit == Card.Suit.Diamond) {
				nextTurn = lastBetPlayer = i;
			}
		}
		NextRound ();
	}

	void NextRound() {
		// Empty Cards Bet
		for (int i = 0; i < bets.Count; ++i){
			for (int c = 0; c <bets[i].cards.Length; ++c){
				Destroy(bets[i].cards[c].gameObject);
			}
		}
		bets.Clear ();

		// Reset all player state
		for (int i = 0; i < players.Count; ++i) {
			players[i].OnTurnEnd();
			players[i].skipSign.SetActive(false);
		}

		skippingPlayer = 0;
		nextTurn = lastBetPlayer;

		// Begin Turn
		NextTurn ();
	}

	public void SetTurn(DeckController deck) {
		for (int i = 0; i < players.Count; ++i) {
			if (players[i] == deck) {
				nextTurn = i;
				break;
			}
		}
	}

	void NextTurn() {
		if (isGameOver)
			return;

		if (bets.Count > 0)
			commentLabel.text = bets.Last ().combination.ToString();

		var current = nextTurn == 0 ? players.Count - 1 : nextTurn - 1;	
		players [current].OnTurnEnd ();

		while (players[nextTurn].Cards.Count <= 0) {
			nextTurn = nextTurn + 1 >= players.Count ? 0 : nextTurn + 1;
		}

		players [nextTurn].OnTurnBegin ();

		nextTurn = nextTurn + 1 >= players.Count ? 0 : nextTurn + 1;
	}

	public void Skip() {
		commentLabel.text = "Pass";
		if (bets.Count > 0) ++skippingPlayer;

		var current = nextTurn == 0 ? players.Count - 1 : nextTurn - 1;	
		Debug.Log (players [current].Cards.Count);
		if (players [current].Cards.Count > 0)
			players [current].skipSign.SetActive (true);

		if (players.Count - skippingPlayer > 1)
			NextTurn ();
		else
			NextRound ();
	}

	public void Finish(DeckController player){
		finishedPlayers.Add (player);

		if (players.Count - finishedPlayers.Count == 1) {
			isGameOver = true;
			Debug.Log("Game Over");
			NextTurn ();
		}
	}
	
	public bool Put (Bet bet){
		if (bet.cards.Length == 0 || bet.combination == Combination.Invalid) {
			commentLabel.text = "Invalid move!";
			return false;
		}

		if (firstTurn && bet.cards [0].Nominal != "3" && bet.cards [0].suit != Card.Suit.Diamond) {
			commentLabel.text = "Must include 3 Diamond";
			return false;
		}
		firstTurn = false;

		// Initialize bet if still empty
		if (bets.Count == 0) {
			bets.Add(bet);
			lastBetPlayer = nextTurn == 0 ? players.Count - 1 : nextTurn - 1;
			NextTurn();
			return true;
		}

		// Get last bet
		var last = bets.Last();

		if (bet.combination <= Combination.Triples && last.combination <= Combination.Triples) {
			// Single, Pair or Triple rule
			if (bet.combination == last.combination && bet.key > last.key) {
				bets.Add(bet);
				lastBetPlayer = nextTurn == 0 ? players.Count - 1 : nextTurn - 1;
				NextTurn();
				return true;
			}
		} else if (bet.combination > Combination.Triples && last.combination > Combination.Triples){
			// 5-card hand
			if (bet.combination > last.combination || (bet.combination == last.combination && bet.key > last.key)) {
				bets.Add(bet);
				lastBetPlayer = nextTurn == 0 ? players.Count - 1 : nextTurn - 1;
				NextTurn();
				return true;
			}
		}
		commentLabel.text = "Too Small!";
		return false;
	}

	static public Bet MakeBet(List<Card> c){
		// Sort card based on nominal
		c.Sort ();

		Bet bet = new Bet ();
		bet.cards = c.ToArray();
		bet.key = c.Last();
		bet.combination = Combination.Invalid;

		switch (c.Count) {
		case 1:
			bet.combination = Combination.Single;
			break;
		case 2:
			if (c[0].Score == c[1].Score)
				bet.combination = Combination.Pairs;
			break;
		case 3:
			if (c[0].Score == c[1].Score && c[1].Score == c[2].Score)
				bet.combination = Combination.Triples;
			break;
		case 5:
			// Check for straight
			if (c[4].Score - c[3].Score == 1 && c[3].Score - c[2].Score == 1 
			    && c[2].Score - c[1].Score == 1 && c[1].Score - c[0].Score == 1)
				bet.combination = Combination.Straight;

			// Check for flush && straight flush
			if (c[0].suit == c[1].suit && c[1].suit == c[2].suit && c[2].suit == c[3].suit && c[3].suit == c[4].suit)
				bet.combination = bet.combination == Combination.Straight ? Combination.StraightFlush : Combination.Flush;
			else if (c[0].Score == c[1].Score && c[1].Score == c[2].Score && c[3].Score == c[4].Score) {
				// fullhouse with 3-2 combination
				bet.combination = Combination.FullHouse;
				bet.key = c[2];
			} else if (c[0].Score == c[1].Score && c[2].Score == c[3].Score && c[3].Score == c[4].Score) {
				// fullhouse with 2-3 combination
				bet.combination = Combination.FullHouse;
				bet.key = c[4];
			} else {
				// Group by suit
 				List<Card> suit_1 = new List<Card>();
				List<Card> suit_2 = new List<Card>();
				for (int i = 0; i < c.Count; ++i){
					if (suit_1.Count == 0) suit_1.Add(c[i]);
					else if (suit_1.Last().suit == c[i].suit) suit_1.Add(c[i]);
					else if (suit_2.Count == 0) suit_2.Add(c[i]);
					else if (suit_2.Last().suit == c[i].suit) suit_2.Add(c[i]);
				}
				suit_1.Sort();
				suit_2.Sort();

				if ((suit_1.Count == 4 && suit_2.Count == 1) || (suit_1.Count == 1 && suit_2.Count == 4)){
					// Four of a Kind
					bet.combination = Combination.FourOfAKind;
					bet.key = suit_1.Count == 4 ? suit_1.Last() : suit_2.Last();
				} 
			}
			break;
		}
		return bet;
	}
}