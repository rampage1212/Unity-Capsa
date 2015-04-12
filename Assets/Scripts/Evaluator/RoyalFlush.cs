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
		evaluator.Evaluate (index);
	}

	protected override void PostEvaluate () {
		evaluator.End ();
		results = evaluator.Results;
	}
}