using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CardSet = System.Collections.Generic.List<Card>;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour {
	public int id = -1;
	PlayerView view;
	CardSet cards;

	// Possible Poker Hand Combinations
	List<PokerHand> hands = new List<PokerHand>(13);

	// Analyze Property
	bool isAnalyzing = false;
	bool analyzeAllMatch = true;
	System.Func<Card, bool> analyzeFilter = null;
	PokerHand.CombinationType analyzeCombination = PokerHand.CombinationType.Invalid;

	public bool Artificial {
		get { return id != 0; }
	}

	public CardSet Cards {
		get { return cards; }
		set {
			cards = value; 
			cards.Sort();
			view.TotalCard = cards.Count;
 			if (!Artificial)
				view.Display (cards);
		}
	}

	public PlayerView View {
		get { return view; }
	}

	void Awake() {
		view = GetComponent<PlayerView> ();
	}

	public void OnTurnBegin() {
		view.OnTurnBegin ();

		var LastTrick = TrickController.Instance.LastTrick;
		if (LastTrick != null) {
			analyzeCombination = LastTrick.Combination;
			analyzeFilter = trick => trick > LastTrick.Key;
		}
		StartCoroutine ("OnTurn");
		StartCoroutine ("Analyze");
	}

	IEnumerator OnTurn() {
		float time = 15f;
		float lastUpdateTime = Time.time;

		while (time > 0f) {
			yield return new WaitForSeconds(0.1f);

			time -= (Time.time - lastUpdateTime);
			lastUpdateTime = Time.time;
			view.TimeLeft = time;

			if (Artificial && !isAnalyzing){
				if (hands.Count > 0) {
					Deal(hands[0]);
				} else {
					yield return new WaitForSeconds(0.5f);
					break;
				}
			}
		}
		
		// Timeout
		Pass ();
	}

	public void OnTurnEnd() {
		StopCoroutine ("Analyze");
		StopCoroutine ("OnTurn");

		view.OnTurnEnd ();
	}

	public void Deal(CardSet dealCards) {
		if (dealCards.Count == 0)
			return;

		PokerHand hand = PokerHand.Make (dealCards);
		if (TrickController.Instance.Deal (hand)) {
			view.OnDealSuccess ();
		} else {
			view.OnDealFailed ();
		}
	}

	public void Deal(PokerHand hand) {
		if (TrickController.Instance.Deal (hand)) {
			view.OnDealSuccess ();
		} else {
			view.OnDealFailed ();
		}	
	}

	public void Pass() {
		TrickController.Instance.Pass ();
		view.OnPass ();
	}

	IEnumerator Analyze() {
		isAnalyzing = true;
		hands.Clear ();
		List<PokerHand> result;

		// One analysis each frame
		switch (analyzeCombination) {
		case PokerHand.CombinationType.Invalid:
			goto case PokerHand.CombinationType.One;
		case PokerHand.CombinationType.One:
			hands.AddRange (One.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));

			if (analyzeCombination == PokerHand.CombinationType.Invalid)
				goto case PokerHand.CombinationType.Pair;
			break;
		case PokerHand.CombinationType.Pair:
			hands.AddRange (Pair.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			
			if (analyzeCombination == PokerHand.CombinationType.Invalid)
				goto case PokerHand.CombinationType.Triple;
			break;
		case PokerHand.CombinationType.Triple:
			hands.AddRange (Triple.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			
			if (analyzeCombination == PokerHand.CombinationType.Invalid)
				goto case PokerHand.CombinationType.Straight;
			break;
		case PokerHand.CombinationType.Straight:
			result = Straight.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter);
			hands.AddRange (result);
			if (result.Count > 0)
				view.Straight = result[result.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.Flush;
		case PokerHand.CombinationType.Flush:
			result = Flush.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter);
			hands.AddRange (result);
			if (result.Count > 0)
				view.Flush = result[result.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.FullHouse;
		case PokerHand.CombinationType.FullHouse:
			result = FullHouse.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter);
			hands.AddRange (result);
			if (result.Count > 0)
				view.FullHouse = result[result.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.FourOfAKind;
		case PokerHand.CombinationType.FourOfAKind:
			result = FourOfAKind.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter);
			hands.AddRange (result);
			if (result.Count > 0)
				view.FourOfAKind = result[result.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.StraightFlush;
		case PokerHand.CombinationType.StraightFlush:
			result = StraightFlush.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter);
			hands.AddRange (result);
			if (result.Count > 0)
				view.StraightFlush = result[result.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.RoyalFlush;
		case PokerHand.CombinationType.RoyalFlush:
			result = RoyalFlush.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter);
			hands.AddRange (result);
			if (result.Count > 0)
				view.RoyalFlush = result[result.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.Dragon;
		case PokerHand.CombinationType.Dragon:
			result = Dragon.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter);
			hands.AddRange (result);
			if (result.Count > 0)
				view.Dragon = result[result.Count - 1].Cards;
			break;
		}

		view.Hint = hands.Count > 0 ? hands[0].Cards : null;
		isAnalyzing = false;
		analyzeFilter = null;
		analyzeCombination = PokerHand.CombinationType.Invalid;
	}
}