using System.Collections;
using System.Collections.Generic;

// 1 Triple + 1 Pair
public class FullHouse : IEvaluator<FullHouse> {
	// Explicitly set triples and pairs to prevent doing another loop check
	public List<CardSet> triples = null;
	public List<CardSet> pairs = null;

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
			pairs = tripleEvaluator.Results;
			triples = pairEvaluator.Results;
		}

		// Build the result
		CardSet fullHouse = new CardSet ();
		for (int tIndex = 0; tIndex < triples.Count; ++tIndex) {
			fullHouse.Clear();
			for (int pIndex = 0; pIndex < pairs.Count; ++pIndex) {
				if (triples[tIndex][0].Nominal != pairs[pIndex][0].Nominal) {
					fullHouse.AddRange(pairs[pIndex]);
					fullHouse.AddRange(triples[tIndex]);
					results.Add(fullHouse);
					break;
				}
			}
		}


		// Reset triples and pairs variable
		triples = null;
		pairs = null;
	}
}