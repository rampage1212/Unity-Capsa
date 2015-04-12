using System.Collections;

// Pair card with same number
public class Pair : IEvaluator<Pair> {
	public override void Evaluate(int index) {
		// Max index is (n-1) - 1, don't evaluate
		if (index > cardSet.Count - 2)
			return;

		Card [] pair = { cardSet [index], cardSet [index + 1] };
		if (pair[0].Nominal == pair[1].Nominal && filter(pair[1])) {
			var set = new CardSet();
			set.AddRange(pair);
			results.Add(new PokerHand(set, set[1], PokerHand.CombinationType.Pair));
		}
	}
}