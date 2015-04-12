using System;
using System.Collections;
using System.Collections.Generic;

public class RoyalFlush : IEvaluator<RoyalFlush> {
	StraightFlush evaluator = new StraightFlush();

	protected override void PreEvaluate () {
		int scoreMin = Array.IndexOf<string>(Card.nominalOrder, "10");
		int scoreMax = Array.IndexOf<string>(Card.nominalOrder, "A");

		evaluator.Begin (cardSet, card => card.Score >= scoreMin && card.Score <= scoreMax);
	}

	public override void Evaluate(int index) {
		base.Evaluate (index);
		evaluator.Evaluate (index);
	}

	protected override void PostEvaluate () {
		evaluator.End ();
		results = evaluator.Results;
	}

	public override bool IsValid(CardSet cards, bool isSorted = false) { 
		if (cards.Count != 5)
			return false;
		if (!isSorted)
			cards.Sort ();
		
		
		return Flush.Instance.IsValid (cards, isSorted)
			&& cards [0].Nominal == "10"
			&& cards [1].Nominal == "J"
			&& cards [2].Nominal == "Q"
			&& cards [3].Nominal == "K"
			&& cards [4].Nominal == "A";
	}
}