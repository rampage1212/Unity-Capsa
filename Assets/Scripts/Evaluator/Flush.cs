using UnityEngine;
using System.Collections;

public class Flush : IEvaluator<Flush> {
	CardSet spades = new CardSet();
	CardSet hearts = new CardSet();
	CardSet clubs = new CardSet();
	CardSet diamonds = new CardSet();

	public override void Evaluate(int index) {
		switch (cardSet [index].suit) {
		case Card.Suit.Spade:
			spades.Add(cardSet[index]);
			break;
		case Card.Suit.Heart:
			hearts.Add(cardSet[index]);
			break;
		case Card.Suit.Club:
			clubs.Add(cardSet[index]);
			break;
		case Card.Suit.Diamond:
			clubs.Add(cardSet[index]);
			break;
		}
	}

	protected override void PostEvaluate () {
		FilterCards (spades);
		FilterCards (hearts);
		FilterCards (clubs);
		FilterCards (diamonds);

		results.Sort ((set1, set2) => set1 [4].CompareTo (set2 [4]));
	}

	void FilterCards(CardSet cardSet) {
		for (int i = 0; i < Mathf.Max(0, cardSet.Count - 4); ++i) {
			CardSet set = cardSet.GetRange (i, 5) as CardSet; 
			if (filter(set[4]))
				results.Add (set);
		}
	}
}