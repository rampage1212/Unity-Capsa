using UnityEngine;
using System.Collections;
using CardSet = System.Collections.Generic.List<Card>;

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
		base.Evaluate (index);

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
			diamonds.Add(cardSet[index]);
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
			CardSet set = cardSet.GetRange (i, 5); 
			if (filter(set[4]))
				results.Add (new PokerHand(set, set[4], PokerHand.CombinationType.Flush));
		}
	}
	
	public override bool IsValid(CardSet cards, bool isSorted = false) { 
		if (cards.Count != 5)
			return false;
		
		return cards[0].suit == cards[1].suit
			&& cards[1].suit == cards[2].suit
			&& cards[2].suit == cards[3].suit
			&& cards[3].suit == cards[4].suit;
	}
}