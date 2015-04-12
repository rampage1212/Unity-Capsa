using System.Collections;

// Three card with same number
public class Triple : IEvaluator<Triple> {
	public override void Evaluate(int index) {
		// Max index is (n-1) - 2, don't evaluate
		if (index > cardSet.Count - 3)
			return;

		// Compare first and last nominal, since it's already sorted
		Card [] triple = { cardSet [index], cardSet [index + 1], cardSet[index + 2] };
		if (triple[0].Nominal == triple[2].Nominal && filter(triple[2])) {
			var set = new CardSet();
			set.AddRange(triple);
			results.Add(new PokerHand(set, set[2], PokerHand.CombinationType.Triple));
		}
	}
}