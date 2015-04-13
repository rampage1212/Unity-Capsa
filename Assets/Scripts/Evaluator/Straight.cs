using System;
using System.Collections;
using System.Collections.Generic;
using CardSet = System.Collections.Generic.List<Card>;

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
		results.Sort ();
	}

	public static void PreEvaluateStraight(ref CardSet bucket, ref CardSet special) {
		bucket.Clear ();
		special.Clear ();
	}

	public static void EvaluateStraight(ref List<PokerHand> results, Card card, ref CardSet bucket, ref CardSet special, Func<Card, bool> filter) {
		if (bucket.Count > 0) {
			int step = card.Score - bucket [bucket.Count - 1].Score;
			
			if (step == 1)
				bucket.Add (card);
			else if (step > 1)
				bucket.Clear ();
			
			// If 5 combination already found, add to result and remove first item for next possibility
			if (bucket.Count == 5) {
				if (filter (bucket [4]))
					results.Add (new PokerHand(new CardSet(bucket), bucket[4], PokerHand.CombinationType.Straight));
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

	public static void PostEvaluateStraight(ref List<PokerHand> results, ref CardSet bucket, ref CardSet special) {
		if (special.Count >= 5 && special [special.Count - 1].Nominal == "2") {
			if (special.Count == 5){
				results.Add(new PokerHand(special, special[4], PokerHand.CombinationType.Straight));
			} else if (special.Count == 6) {
				var set = new CardSet();

				// 2-3-4-5-6
				set.Add(special[special.Count - 1]); // 2
				set.AddRange(special.GetRange(0, 4)); // 3-4-5-6
				results.Add(new PokerHand(set, set[0], PokerHand.CombinationType.Straight));

				// A-2-3-4-5
				set = special.GetRange(special.Count - 2, 2); // A-2
				set.AddRange(special.GetRange(0, 3)); // 3-4-5
				results.Add(new PokerHand(set, set[1], PokerHand.CombinationType.Straight));
			}
		}
	}

	public override bool IsValid(CardSet cards, bool isSorted = false) { 
		if (cards.Count != 5)
			return false;

		if (!isSorted) {
			// Special Case
			if (cards [0].Nominal == "2" 
				&& cards [1].Nominal == "3" 
				&& cards [2].Nominal == "4" 
				&& cards [3].Nominal == "5" 
				&& cards [4].Nominal == "6")
				return true;

			if (cards [0].Nominal == "A" 
				&& cards [1].Nominal == "2" 
				&& cards [2].Nominal == "3" 
				&& cards [3].Nominal == "4" 
				&& cards [4].Nominal == "5")
				return true;

			cards.Sort ();
		} else {
			// Special Case
			if (cards [0].Nominal == "3" 
			    && cards [1].Nominal == "4" 
			    && cards [2].Nominal == "5" 
			    && cards [3].Nominal == "6" 
			    && cards [4].Nominal == "2")
				return true;
			
			if (cards [0].Nominal == "3" 
			    && cards [1].Nominal == "4" 
			    && cards [2].Nominal == "5" 
			    && cards [3].Nominal == "A" 
			    && cards [4].Nominal == "2")
				return true;
		}

		// Normal case
		for (int i = 0; i < cards.Count - 1; ++i) {
			if (cards[i].Score + 1 != cards[i + 1].Score)
				return false;
		}
		
		return true;
	}
}