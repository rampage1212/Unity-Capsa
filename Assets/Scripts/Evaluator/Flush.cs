using UnityEngine;
using System.Collections;

public class Flush : IEvaluator<Flush> {
	CardSet spades = new CardSet();
	CardSet hearts = new CardSet();
	CardSet clubs = new CardSet();
	CardSet diamonds = new CardSet();

	protected override void PreEvaluate () {
		spades.Clear ();
		hearts.Clear ();
		clubs.Clear ();
		diamonds.Clear ();
	}

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

		results.Sort ();
	}

	void FilterCards(CardSet cardSet) {
		for (int i = 0; i < Mathf.Max(0, cardSet.Count - 4); ++i) {
			CardSet set = cardSet.GetRange (i, 5) as CardSet; 
			if (filter(set[4]))
				results.Add (new PokerHand(set, set[4], PokerHand.CombinationType.Flush));
		}
	}
}