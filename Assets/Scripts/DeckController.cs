using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DeckController : MonoBehaviour {
	// Passed by BigTwo class
	public BigTwo bigTwo;
	public int playerId;
	public bool artificial = true;

	public GameObject control;
	public GameObject skipSign;
	public Text timerLabel;
	public Text cardCountLabel;
	public QuickBarHelper bar;
	
	// Collection of card
	List<Card> _cards = new List<Card>();
	List<Card> selectedCards = new List<Card>();
	List<Card> spades = new List<Card> ();
	List<Card> hearts = new List<Card> ();
	List<Card> clubs = new List<Card> ();
	List<Card> diamonds = new List<Card> ();

	// Card Combination
	BigTwo.Combination lastGroupSelect = BigTwo.Combination.Invalid;
	List<Card> pair = new List<Card> ();
	List<Card> triple = new List<Card> ();
	List<Card> straight = new List<Card>();
	List<Card> flush = new List<Card>();
	List<Card> fullHouse = new List<Card>();
	List<Card> fourOfAKind = new List<Card>();
	List<Card> straightFlush = new List<Card>();
	List<Card> royalFlush = new List<Card> ();
	List<Card> dragon = new List<Card> ();

	List<Card> hint = new List<Card> ();

	public List<Card> Cards {
		get { return _cards; }
		set {
			// Assing and sort card
			_cards = value;
			_cards.Sort ();

			// Group card
			for (int i = 0; i < _cards.Count; ++i){
				switch (_cards[i].suit){
				case Card.Suit.Spade:
					spades.Add(_cards[i]);
					break;
				case Card.Suit.Heart:
					hearts.Add(_cards[i]);
					break;
				case Card.Suit.Club:
					clubs.Add(_cards[i]);
					break;
				case Card.Suit.Diamond:
					diamonds.Add(_cards[i]);
					break;
				}
			}

			// Show card if not computer controlled
			if (!artificial) UpdateDisplay();

			// Update count label
			if (cardCountLabel)
				cardCountLabel.text = "" + _cards.Count;
		}
	}

	public void Select(Card c){
		selectedCards.Add (c);
		lastGroupSelect = BigTwo.Combination.Invalid;
	}

	public void Deselect(Card c){
		selectedCards.Remove (c);
		lastGroupSelect = BigTwo.Combination.Invalid;
	}

	public void Out(){
		if (selectedCards.Count == 0)
			return;

		BigTwo.Bet bet = BigTwo.MakeBet (selectedCards);

		if (bigTwo.Put (bet)) {
			for (int i = 0; i < selectedCards.Count; ++i) {
				selectedCards [i].transform.SetParent (bigTwo.transform.GetChild (playerId));
				selectedCards [i].button.interactable = false;
				_cards.Remove (selectedCards [i]);

				// remove from group
				switch (selectedCards [i].suit) {
				case Card.Suit.Spade:
					spades.Remove (selectedCards [i]);
					break;
				case Card.Suit.Heart:
					hearts.Remove (selectedCards [i]);
					break;
				case Card.Suit.Club:
					clubs.Remove (selectedCards [i]);
					break;
				case Card.Suit.Diamond:
					diamonds.Remove (selectedCards [i]);
					break;
				}
			}
			selectedCards.Clear ();
			
			if (_cards.Count == 0)
				bigTwo.Finish(this);
		}
	}

	void UpdateDisplay(){
		for (int i = 0; i < _cards.Count; ++i){
			_cards[i].transform.SetParent(transform, false);
		}
	}

	IEnumerator OnTurn(){
		yield return null;

		float time = 15f;
		hint = CalculateMove ();

		while (time > 0f) {
			time -= Time.deltaTime;
			if (timerLabel) timerLabel.text = "" + Mathf.CeilToInt(time);
			if (artificial) {
				selectedCards = hint;

				if (hint.Count == 0) {
					yield return new WaitForSeconds(Random.Range(0.5f, 1f));
					bigTwo.Skip();
				} else {
					if (!IsInvoking("Out")) {
						Invoke("Out", Random.Range(hint.Count * 0.5f, hint.Count * 2.5f));
					}
				}
			}
			yield return null;
		}

		// Timeout
		bigTwo.Skip ();
	}

	public void OnTurnBegin(){
		if (control)
			control.SetActive (true);

		UpdateHint ();
		skipSign.SetActive(false);
		
		StartCoroutine ("OnTurn");
	}

	public void OnTurnEnd(){
		if (control)
			control.SetActive (false);

		if (bar) 
			bar.Interactable = false;

		if (cardCountLabel)
			cardCountLabel.text = "" + _cards.Count;
		
		StopCoroutine ("OnTurn");
	}

	void UpdateHint(){
		pair.Clear ();
		triple.Clear ();
		straight.Clear ();
		flush.Clear ();
		fullHouse.Clear ();
		fourOfAKind.Clear ();
		straightFlush.Clear ();
		royalFlush.Clear ();
		dragon.Clear ();

		// Iterate from higher card to lower cards
		for (int i = _cards.Count - 1; i >= 0; --i) {
			// Check for pair
			if (pair.Count == 0 && i > 0) {
				Card [] c = {_cards [i - 1], _cards [i]};
				if (c [0].Nominal == c [1].Nominal) {
					pair.AddRange (c);
				}
			}

			// Check for triple
			if (triple.Count == 0 && i > 1) {
				Card [] c = {_cards [i - 2], _cards [i - 1], _cards[i]};
				if (c [0].Nominal == c [2].Nominal) {
					triple.AddRange (c);
				}
			}
				
				// Check for straight
			if (straight.Count == 0){
				straight.Add(_cards[i]);
			} else if (straight.Count < 5) {
				var step = straight.Last().Score - _cards[i].Score;
				if (step > 1) straight.Clear();
				else if (step == 1) straight.Add(_cards[i]);
			}

			// Straight flush
			if (straightFlush.Count == 0){
				straightFlush.Add(_cards[i]);
			} else if (straightFlush.Count < 5) {
				var step = straightFlush.Last().Score - _cards[i].Score;
				if (step > 1) straightFlush.Clear();
				else if (step == 1 && straightFlush.Last().suit == _cards[i].suit) straightFlush.Add(_cards[i]);
			}

			// TODO : Check for full house
			if (fullHouse.Count == 0){
				fullHouse.Add(_cards[i]);
			} else if (fullHouse.Count < 5) {
				var step = fullHouse.Last().Score - _cards[i].Score;
				if (step == 0) {
					fullHouse.Add(_cards[i]);
				} else {
					if (fullHouse.Count < 2) {
						fullHouse.Clear();
						fullHouse.Add(_cards[i]);
					} else if (fullHouse.Count > 3) {
						fullHouse.RemoveRange(3, fullHouse.Count - 3);
					} else if (fullHouse.Count == 2) {
						fullHouse.Add(_cards[i]);
					} else if (fullHouse.Count == 3) {
						if (fullHouse[1].Score == fullHouse[2].Score)
							fullHouse.Add(_cards[i]);
						else
							fullHouse.RemoveAt(2);
					} 
				}
			}

			// Check for dragon
			if (dragon.Count == 0 || (dragon.Last().Score - _cards[i].Score == 1)){
				dragon.Add(_cards[i]);
			}
		}

		if (dragon.Count != 13)
			dragon.Clear ();

		if (straight.Count == 5) {
			if (bar) bar.straight.interactable = true;
		}else {
			if (bar) bar.straight.interactable = false;
			straight.Clear ();
		}

		if (straightFlush.Count == 5) {
			if (bar) bar.straightFlush.interactable = true;
		}else {
			if (bar) bar.straightFlush.interactable = false;
			straightFlush.Clear ();
		}

		if (fullHouse.Count == 5) {
			if (bar) bar.fullHouse.interactable = true;
		} else {
			if (bar) bar.fullHouse.interactable = false;
			fullHouse.Clear();
		}

		// Check for flush
		if (spades.Count >= 5) {
			flush = spades.GetRange(spades.Count - 5, 5);
		}
		if (hearts.Count >= 5) {
			if (flush.Count == 0 || (flush.Count > 0 && flush.Last() < hearts.Last())) 
				flush = hearts.GetRange(hearts.Count - 5, 5);
		}
		if (clubs.Count >= 5) {
			if (flush.Count == 0 || (flush.Count > 0 && flush.Last() < clubs.Last()))
				flush = clubs.GetRange(clubs.Count - 5, 5);
		}
		if (diamonds.Count >= 5) {
			if (flush.Count == 0 || (flush.Count > 0 && flush.Last() < diamonds.Last()))
				flush = diamonds.GetRange(diamonds.Count - 5, 5);
		}
		if (flush.Count == 5) {
			if (bar) bar.flush.interactable = true;
		}

		
		// Check for four of a kind
		if (spades.Count >= 4) {
			fourOfAKind = spades.GetRange(spades.Count - 4, 4);
		}
		if (hearts.Count >= 4) {
			if (fourOfAKind.Count == 0 || (fourOfAKind.Count > 0 && fourOfAKind.Last() < hearts.Last())) 
				fourOfAKind = hearts.GetRange(hearts.Count - 4, 4);
		}
		if (clubs.Count >= 4) {
			if (fourOfAKind.Count == 0 || (fourOfAKind.Count > 0 && fourOfAKind.Last() < clubs.Last()))
				fourOfAKind = clubs.GetRange(clubs.Count - 4, 4);
		}
		if (diamonds.Count >= 4) {
			if (fourOfAKind.Count == 0 || (fourOfAKind.Count > 0 && fourOfAKind.Last() < diamonds.Last()))
				fourOfAKind = diamonds.GetRange(diamonds.Count - 4, 4);
		}
		if (fourOfAKind.Count == 4) {
			// get the a kind
			for (int i = 0; i < _cards.Count; ++i){
				if (_cards[i].Score != fourOfAKind[0].Score && _cards[i].suit != fourOfAKind[0].suit) {
					fourOfAKind.Add(_cards[i]);
					if (bar) bar.fourOfAKind.interactable = true;
					break;
				}
			}
		}

		// Check for royal flush
		if (flush.Count == 5) {
			if (flush[0].Nominal == "10" && flush[1].Nominal == "J" && flush[2].Nominal == "Q" && flush[3].Nominal == "K" && flush[4].Nominal == "A") {
				royalFlush = flush;
				if (bar) bar.royalFlush.interactable = true;
			}
		}
	}

	// Simple calculation for possible card combination
	public List<Card> CalculateMove(){
		hint.Clear ();
		List<Card> result = new List<Card> ();

		if (bigTwo.bets.Count == 0) {
			bool firstTrick = _cards.Count == 13;

			// Lambda expression to determine possible card
			System.Func<List<Card>, bool> checkCombination = delegate(List<Card> combination) {
				return (combination.Count > 0) && (!firstTrick || (firstTrick && combination[0].Nominal == "3" && combination[0].suit == Card.Suit.Diamond));
			};

			if (dragon.Count > 0)
				result = dragon;
			else if (royalFlush.Count > 0 && !firstTrick) {
				result = royalFlush; 
				Debug.Log("Royal flush");
			} else if (checkCombination(straightFlush)) {
				result = straightFlush;
				Debug.Log("Straight flush");
			} else if (checkCombination(fourOfAKind)) {
				result = fourOfAKind;
				Debug.Log("Four of a kind");
			} else if (checkCombination(fullHouse)) {
				result = fullHouse;
				Debug.Log("Full house");
			} else if (checkCombination(flush)) {
				result = flush;
				Debug.Log("Flush");
			} else if (checkCombination(straight)) {
				result = straight;
				Debug.Log("Straight");
			} else if (checkCombination(triple)) {
				result = triple;
				Debug.Log("Triple");
			} else if (checkCombination(pair)) {
				result = pair;
				Debug.Log("Pair");
			} else {
				result.Add(_cards[0]);
				Debug.Log("Single");
			}
		} else {
			BigTwo.Bet lastBet = bigTwo.bets.Last ();
			switch (lastBet.combination) {
			case BigTwo.Combination.Single:
				for (int i = 0; i < _cards.Count; ++i) {
					var c = _cards [i];
					if (c > lastBet.key) {
						result.Add (c);
						break;
					}
				}
				break;
			case BigTwo.Combination.Pairs:
				for (int i = 0; i < _cards.Count - 1; ++i) {
					Card [] c = {_cards [i], _cards [i + 1]};
					if (c [0].Nominal == c [1].Nominal && c [1] > lastBet.key) {
						result.AddRange (c);
						break;
					}
				}
				break;
			case BigTwo.Combination.Triples:
				for (int i = 0; i < _cards.Count - 2; ++i) {
					Card [] c = {_cards [i], _cards [i + 1], _cards [i + 2]};
					if (c [0].Nominal == c [1].Nominal && c [1].Nominal == c [2].Nominal && c [2] > lastBet.key) {
						result.AddRange (c);
						break;
					}
				}
				break;
			case BigTwo.Combination.Straight:
				if (straight.Count > 0 && straight.Last () > lastBet.key) {
					result = straight;
					break;
				}
				goto case BigTwo.Combination.Flush;
			case BigTwo.Combination.Flush:
				if (flush.Count > 0 && flush.Last () > lastBet.key) {
					result = flush;
					break;
				}
				goto case BigTwo.Combination.FullHouse;
			case BigTwo.Combination.FullHouse:
				if (fullHouse.Count > 0) {
					if ((fullHouse [0].suit == fullHouse [2].suit && fullHouse [2] > lastBet.key)
						|| (fullHouse [2].suit == fullHouse [4].suit && fullHouse [4] > lastBet.key)) {
						result = fullHouse;
						break;
					}
				}
				goto case BigTwo.Combination.FourOfAKind;
			case BigTwo.Combination.FourOfAKind:
				if (fourOfAKind.Count > 0) {
					for (int i = fourOfAKind.Count - 1; i > 0; --i) {
						if (fourOfAKind [i].suit == fourOfAKind [i - 1].suit) {
							if (fourOfAKind [i] > lastBet.key)
								result = fourOfAKind;
							break; // break for loop
						}
					}
				}
				goto case BigTwo.Combination.StraightFlush;
			case BigTwo.Combination.StraightFlush:
				if (straightFlush.Count > 0 && straightFlush.Last () > lastBet.key) {
					result = straightFlush;
					break;
				}
				goto case BigTwo.Combination.RoyalFlush;
			case BigTwo.Combination.RoyalFlush:
				if (royalFlush.Count > 0 && royalFlush.Last () > lastBet.key) {
					result = royalFlush;
				}
				break;
			}
		}

		return result;
	}

	// Event for Button
	public void OnSelectStraight() {
		SelectGroup (straight, lastGroupSelect != BigTwo.Combination.Straight);
		lastGroupSelect = BigTwo.Combination.Straight;
	}
	
	public void OnSelectFlush() {
		SelectGroup (flush, lastGroupSelect != BigTwo.Combination.Flush);
		lastGroupSelect = BigTwo.Combination.Flush;
	}
	
	public void OnSelectFullHouse() {
		SelectGroup (fullHouse, lastGroupSelect != BigTwo.Combination.FullHouse);
		lastGroupSelect = BigTwo.Combination.FullHouse;
	}
	
	public void OnSelectFourOfAKind() {
		SelectGroup (fourOfAKind, lastGroupSelect != BigTwo.Combination.FourOfAKind);
		lastGroupSelect = BigTwo.Combination.FourOfAKind;
	}
	
	public void OnSelectStraightFlush() {
		SelectGroup (straightFlush, lastGroupSelect != BigTwo.Combination.StraightFlush);
		lastGroupSelect = BigTwo.Combination.StraightFlush;
	}

	public void OnSelectRoyalFlush() {
		SelectGroup (royalFlush, lastGroupSelect != BigTwo.Combination.RoyalFlush);
		lastGroupSelect = BigTwo.Combination.RoyalFlush;
	}

	public void OnSelectDragon() {
		SelectGroup (dragon, lastGroupSelect != BigTwo.Combination.Dragon);
		lastGroupSelect = BigTwo.Combination.Dragon;
	}

	public void OnSelectHint() {
		SelectGroup (hint, true);
		lastGroupSelect = BigTwo.Combination.Invalid;
	}

	// Helper for group select
	void SelectGroup(List<Card> l, bool reset) {
		// Deselect all
		if (reset) {
			for (int i = 0; i < _cards.Count; ++i) {
				_cards [i].Select (false);
			}
		}

		// Select new card
		for (int i = 0; i < l.Count; ++i) {
			l[i].ToggleSelect();
		}
	}
}