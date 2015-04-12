using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour {
	PlayerView view;
	CardSet cards;

	// Possible Poker Hand Combinations
	List<PokerHand> hands = new List<PokerHand>(13);

	// Analyze Property
	bool isAnalyzing = false;
	bool analyzeAllMatch = true;
	System.Func<Card, bool> analyzeFilter = null;
	PokerHand.CombinationType analyzeCombination = PokerHand.CombinationType.Invalid;

	void Awake() {
		view = GetComponent<PlayerView> ();
	}

	public CardSet Cards {
		set { 
			cards = value; 
			cards.Sort();
			view.TotalCard = cards.Count;
			view.Display (cards);
		}
	}

	IEnumerator Analyze() {
		isAnalyzing = true;
		hands.Clear ();

		// One analysis each frame
		switch (analyzeCombination) {
		case PokerHand.CombinationType.Single:
			hands.AddRange (Single.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			break;
		case PokerHand.CombinationType.Pair:
			hands.AddRange (Pair.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			break;
		case PokerHand.CombinationType.Triple:
			hands.AddRange (Triple.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			break;
		case PokerHand.CombinationType.Straight:
			hands.AddRange (Triple.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				view.Straight = hands[hands.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.Flush;
		case PokerHand.CombinationType.Flush:
			hands.AddRange (Flush.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				view.Flush = hands[hands.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.FullHouse;
		case PokerHand.CombinationType.FullHouse:
			hands.AddRange (FullHouse.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				view.FullHouse = hands[hands.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.FourOfAKind;
		case PokerHand.CombinationType.FourOfAKind:
			hands.AddRange (FourOfAKind.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				view.FourOfAKind = hands[hands.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.StraightFlush;
		case PokerHand.CombinationType.StraightFlush:
			hands.AddRange (StraightFlush.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				view.StraightFlush = hands[hands.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.RoyalFlush;
		case PokerHand.CombinationType.RoyalFlush:
			hands.AddRange (RoyalFlush.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				view.RoyalFlush = hands[hands.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.Dragon;
		case PokerHand.CombinationType.Dragon:
			hands.AddRange (Dragon.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				view.Dragon = hands[hands.Count - 1].Cards;
			break;
		}

		view.Hint = hands.Count > 0 ? hands[0].Cards : null;
	}
}