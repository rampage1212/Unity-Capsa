using System.Collections;

public class One : IEvaluator<One> {
	public override void Evaluate(int index) {
		if (filter(cardSet[index])) {
			var set = new CardSet();
			set.Add(cardSet[index]);
			results.Add(new PokerHand(set, set[0], PokerHand.CombinationType.One));
		}
	}

	public override bool IsValid(CardSet cards, bool isSorted = false) { 
		return cards.Count == 1;
	}
}