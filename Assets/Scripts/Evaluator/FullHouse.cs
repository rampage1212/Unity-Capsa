using System.Collections;
using System.Collections.Generic;
using CardSet = System.Collections.Generic.List<Card>;

// 1 Triple + 1 Pair
public class FullHouse : IEvaluator<FullHouse> {
	// Explicitly set triples and pairs to prevent doing another loop check
	public List<PokerHand> triples = null;
	public List<PokerHand> pairs = null;

	Triple tripleEvaluator = new Triple();
	Pair pairEvaluator = new Pair();

	protected override void PreEvaluate () {
		// Do nothing if triples and pairs is specified manually
		if (triples != null || pairs != null)
			return;

		// Begin Pair and Triple evaluation
		tripleEvaluator.Begin (cardSet, filter);
		pairEvaluator.Begin (cardSet, filter);
	}

	public override void Evaluate(int index) {
		base.Evaluate (index);

		// Do nothing if triples and pairs is specified manually
		if (triples != null || pairs != null)
			return;

		// Evaluate each card
		tripleEvaluator.Evaluate (index);
		pairEvaluator.Evaluate (index);
	}

	protected override void PostEvaluate () {
		// Do nothing if triples and pairs is specified manually
		if (triples == null && pairs == null) {
			// End Pair and Triple evaluation
			tripleEvaluator.End ();
			pairEvaluator.End ();

			// Get the result of the evaluation
			triples = tripleEvaluator.Results;
			pairs = pairEvaluator.Results;
		}

		// Build the result
		CardSet fullHouse = new CardSet (5);
		for (int tIndex = 0; tIndex < triples.Count; ++tIndex) {
			fullHouse.Clear();
			for (int pIndex = 0; pIndex < pairs.Count; ++pIndex) {
				if (triples[tIndex].Cards[0].Nominal != pairs[pIndex].Cards[0].Nominal) {
					fullHouse.AddRange(pairs[pIndex].Cards);
					fullHouse.AddRange(triples[tIndex].Cards);
					results.Add(new PokerHand(fullHouse, triples[tIndex].Cards[2], PokerHand.CombinationType.FullHouse));
					break;
				}
			}
		}


		// Reset triples and pairs variable
		triples = null;
		pairs = null;
	}
	
	public override bool IsValid(CardSet cards, bool isSorted = false) { 
		if (cards.Count != 5)
			return false;
	
		if (!isSorted)
			cards.Sort ();
		
		return cards[0].Nominal == cards[1].Nominal && cards[1].Nominal == cards[2].Nominal && cards[3].Nominal == cards[4].Nominal
			|| cards[0].Nominal == cards[1].Nominal && cards[2].Nominal == cards[3].Nominal && cards[3].Nominal == cards[4].Nominal;
	}
}