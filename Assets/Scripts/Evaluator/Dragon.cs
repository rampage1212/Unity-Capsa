using System.Collections.Generic;

// 13 card straight in order
public class Dragon : IEvaluator<Dragon> {
	CardSet dragon = new CardSet();

	protected override void PreEvaluate () {
		dragon.Clear ();
		dragon.Add (cardSet [0]);
	}

	public override void Evaluate(int index) {
		base.Evaluate (index);

		if (dragon[dragon.Count - 1].Score + 1 == cardSet[index].Score) {
			dragon.Add (cardSet [index]);
		}
	}

	protected override void PostEvaluate () {
		if (dragon.Count == 13)
			results.Add (new PokerHand(dragon, dragon[dragon.Count - 1]	, PokerHand.CombinationType.Dragon));
	}

	public override bool IsValid(CardSet cards, bool isSorted = false) { 
		if (cards.Count != 13)
			return false;

		if (!isSorted)
			cards.Sort ();

		for (int i = 0; i < cards.Count - 1; ++i) {
			if (cards[i].Score + 1 != cards[i + 1].Score)
				return false;
		}

		return true;
	}
}