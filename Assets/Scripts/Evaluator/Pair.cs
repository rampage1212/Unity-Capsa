using System.Collections;
using CardSet = System.Collections.Generic.List<Card>;

// Pair card with same number
public class Pair : IEvaluator<Pair> {
	public override void Evaluate(int index) {
		base.Evaluate (index);

		// Max index is (n-1) - 1, don't evaluate
		if (index > cardSet.Count - 2)
			return;

		Card [] pair = { cardSet [index], cardSet [index + 1] };
		if (pair[0].Score == pair[1].Score && filter(pair[1])) {
			results.Add(new PokerHand(new CardSet(pair), pair[1], PokerHand.CombinationType.Pair));
		}
	}

	public override bool IsValid(CardSet cards, bool isSorted = false) { 
		if (cards.Count != 2)
			return false;

		return cards [0].Score == cards [1].Score;
	}
}