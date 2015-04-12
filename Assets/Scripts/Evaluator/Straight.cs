using System.Collections;

public class Straight : IEvaluator<Straight> {
	CardSet bucket = new CardSet();
	CardSet special = new CardSet(); // A-2-3-4-5 or 2-3-4-5-6 Straight

	protected override void PreEvaluate () {
		bucket.Clear ();
		bucket.Add (cardSet [0]);

		special.Clear ();
		if (cardSet [0].Nominal == "3")
			special.Add (cardSet [0]);
	}

	public override void Evaluate(int index) {
		int step = cardSet [index].Score - bucket [bucket.Count - 1].Score;

		if (step == 1)
			bucket.Add (cardSet [index]);
		else if (step > 1)
			bucket.Clear();

		// If 5 combination already found, add to result and remove first item for next possibility
		if (bucket.Count == 5) {
			if (filter(bucket[4]))
				results.Add(bucket);
			bucket.RemoveAt(0);
		}

		// Special Straight evaluation
		if (special.Count == 0)
			return;
		var curr_n = cardSet [index].Nominal;
		var prev_n = special [special.Count - 1].Nominal;
		if (prev_n == "3" && curr_n == "4" 
		    || prev_n == "4" && curr_n == "5" 
		    || prev_n == "5" && curr_n == "6"
		    || ((prev_n == "5" || prev_n == "6")
		    && (curr_n == "A" || curr_n == "2")))
			special.Add(cardSet[index]);
	}

	protected override void PostEvaluate () {
		if (special.Count >= 5 && special [special.Count - 1].Nominal == "2") {
			if (special.Count == 5){
				results.Add(special);
			} else if (special.Count == 6) {
				// 2-3-4-5-6
				CardSet set = special.GetRange(0, 4) as CardSet;
				set.Add(special[special.Count - 1]);
				results.Add(set);

				// A-2-3-4-5
				set = special.GetRange(0, 3) as CardSet;
				set.Add(special[special.Count - 2]);
				set.Add(special[special.Count - 1]);
				results.Add(set);
			}
		}
	}
}