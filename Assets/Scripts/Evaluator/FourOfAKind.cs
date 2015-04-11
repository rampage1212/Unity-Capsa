using System.Collections;

public class FourOfAKind : IEvaluator<FourOfAKind> {
	public override void Evaluate(int index) {
		// 5 card is needed at least and Max index is (n-1) - 3, don't evaluate
		if (cardSet.Count < 5 || index > cardSet.Count - 3)
			return;

		// Compare first and last nominal, since it's already sorted
		Card [] quad = { cardSet [index], cardSet [index + 1], cardSet[index + 2], cardSet[index + 3] };
		if (quad[0].Nominal == quad[3].Nominal && filter(quad[3])) {
			var set = new CardSet();
			set.Add(cardSet[index == 0 ? 4 : 0]);
			set.AddRange(quad);
			results.Add(set);
		}
	}
}