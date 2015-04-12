using System.Collections.Generic;

// 13 card straight in order
public class Dragon : IEvaluator<Dragon> {
	CardSet dragon = new CardSet();

	protected override void PreEvaluate () {
		dragon.Clear ();
		dragon.Add (cardSet [0]);
	}

	public override void Evaluate(int index) {
		if (dragon[dragon.Count - 1].Score + 1 == cardSet[index].Score) {
			dragon.Add (cardSet [index]);
		}
	}

	protected override void PostEvaluate () {
		if (dragon.Count == 13)
			results.Add (dragon);
	}
}