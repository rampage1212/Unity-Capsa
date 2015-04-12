using System;
using System.Collections;
using System.Collections.Generic;

public class Straight : IEvaluator<Straight> {
	CardSet bucket = new CardSet();
	CardSet special = new CardSet(); // A-2-3-4-5 or 2-3-4-5-6 Straight

	protected override void PreEvaluate () {
		PreEvaluateStraight (ref bucket, ref special);
	}

	public override void Evaluate(int index) {
		EvaluateStraight (ref results, cardSet [index], ref bucket, ref special, filter);
	}

	protected override void PostEvaluate () {
		PostEvaluateStraight (ref results, ref bucket, ref special);
		results.Sort ((set1, set2) => set1 [4].CompareTo (set2 [4]));
	}

	public static void PreEvaluateStraight(ref CardSet bucket, ref CardSet special) {
		bucket.Clear ();
		special.Clear ();
	}

	public static void EvaluateStraight(ref List<CardSet> results, Card card, ref CardSet bucket, ref CardSet special, Func<Card, bool> filter) {
		if (bucket.Count > 0) {
			int step = card.Score - bucket [bucket.Count - 1].Score;
			
			if (step == 1)
				bucket.Add (card);
			else if (step > 1)
				bucket.Clear ();
			
			// If 5 combination already found, add to result and remove first item for next possibility
			if (bucket.Count == 5) {
				if (filter (bucket [4]))
					results.Add (bucket);
				bucket.RemoveAt (0);
			}
		} else {
			bucket.Add(card);
		}
		
		// Special Straight evaluation
		if (special.Count == 0 && card.Nominal == "3")
			special.Add (card);

		if (special.Count > 0) {
			var curr_n = card.Nominal;
			var prev_n = special [special.Count - 1].Nominal;
			if (prev_n == "3" && curr_n == "4" 
				|| prev_n == "4" && curr_n == "5" 
				|| prev_n == "5" && curr_n == "6"
				|| ((prev_n == "5" || prev_n == "6")
				&& (curr_n == "A" || curr_n == "2")))
				special.Add (card);
		}
	}

	public static void PostEvaluateStraight(ref List<CardSet> results, ref CardSet bucket, ref CardSet special) {
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