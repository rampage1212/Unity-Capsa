using System.Collections;
using System.Collections.Generic;

// 1 Triple + 1 Pair
public class FullHouse : IEvaluator<FullHouse> {
	// Explicitly set triples and pairs to prevent doing another loop check
	public List<CardSet> triples = null;
	public List<CardSet> pairs = null;

	protected override void PreEvaluate () {
		// Do nothing if triples and pairs is specified manually
		if (triples == null && pairs == null)
			return;

		// Begin Pair and Triple evaluation
		Pair.Instance.Begin (cardSet, filter);
		Triple.Instance.Begin (cardSet, filter);
	}

	public override void Evaluate(int index) {
		// Do nothing if triples and pairs is specified manually
		if (triples == null && pairs == null)
			return;

		// Evaluate each card
		Pair.Instance.Evaluate (index);
		Triple.Instance.Evaluate (index);
	}

	protected override void PostEvaluate () {
		// Do nothing if triples and pairs is specified manually
		if (triples == null && pairs == null) {
			// End Pair and Triple evaluation
			Pair.Instance.End ();
			Triple.Instance.End ();
		}

		pairs = Pair.Instance.Results;
		triples = Triple.Instance.Results;

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