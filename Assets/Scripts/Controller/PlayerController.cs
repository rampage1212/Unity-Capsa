using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerHelper))]
public class PlayerController : MonoBehaviour {
	PlayerHelper helper;
	CardSet cards;

	// Possible Poker Hand Combinations
	List<PokerHand> hands = new List<PokerHand>(13);

	// Analyze Property
	bool isAnalyzing = false;
	bool analyzeAllMatch = true;
	System.Func<Card, bool> analyzeFilter = null;
	PokerHand.CombinationType analyzeCombination = PokerHand.CombinationType.Invalid;

	void Awake() {
		helper = GetComponent<PlayerHelper> ();
	}

	public CardSet Cards {
		set { 
			cards = value; 
			cards.Sort();
			helper.TotalCard = cards.Count;
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
				helper.Straight = hands[hands.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.Flush;
		case PokerHand.CombinationType.Flush:
			hands.AddRange (Flush.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				helper.Flush = hands[hands.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.FullHouse;
		case PokerHand.CombinationType.FullHouse:
			hands.AddRange (FullHouse.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				helper.FullHouse = hands[hands.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.FourOfAKind;
		case PokerHand.CombinationType.FourOfAKind:
			hands.AddRange (FourOfAKind.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				helper.FourOfAKind = hands[hands.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.StraightFlush;
		case PokerHand.CombinationType.StraightFlush:
			hands.AddRange (StraightFlush.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				helper.StraightFlush = hands[hands.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.RoyalFlush;
		case PokerHand.CombinationType.RoyalFlush:
			hands.AddRange (RoyalFlush.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				helper.RoyalFlush = hands[hands.Count - 1].Cards;
			if (!analyzeAllMatch && hands.Count > 0)
				break;
			yield return null;
			goto case PokerHand.CombinationType.Dragon;
		case PokerHand.CombinationType.Dragon:
			hands.AddRange (Dragon.Instance.LazyEvaluator (cards, analyzeAllMatch, analyzeFilter));
			if (hands.Count > 0)
				helper.Dragon = hands[hands.Count - 1].Cards;
			break;
		}

		helper.Hint = hands.Count > 0 ? hands[0].Cards : null;
	}
}