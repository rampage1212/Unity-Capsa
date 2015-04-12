using System.Collections;

public class StraightFlush : IEvaluator<StraightFlush> {
	CardSet bucketSpades = new CardSet();
	CardSet specialSpades = new CardSet();

	CardSet bucketHearts = new CardSet();
	CardSet specialHearts = new CardSet();

	CardSet bucketClubs = new CardSet();
	CardSet specialClubs = new CardSet();

	CardSet bucketDiamonds = new CardSet();
	CardSet specialDiamonds = new CardSet();

	protected override void PreEvaluate () {
		Straight.PreEvaluateStraight (ref bucketSpades, ref specialSpades);
		Straight.PreEvaluateStraight (ref bucketHearts, ref specialHearts);
		Straight.PreEvaluateStraight (ref bucketClubs, ref specialClubs);
		Straight.PreEvaluateStraight (ref bucketDiamonds, ref specialDiamonds);
	}
	
	public override void Evaluate(int index) {
		switch (cardSet [index].suit) {
		case Card.Suit.Spade:
			Straight.EvaluateStraight(ref results, cardSet[index], ref bucketSpades, ref specialSpades, filter);
			break;
		case Card.Suit.Heart:
			Straight.EvaluateStraight(ref results, cardSet[index], ref bucketHearts, ref specialHearts, filter);
			break;
		case Card.Suit.Club:
			Straight.EvaluateStraight(ref results, cardSet[index], ref bucketClubs, ref specialClubs, filter);
			break;
		case Card.Suit.Diamond:
			Straight.EvaluateStraight(ref results, cardSet[index], ref bucketDiamonds, ref specialDiamonds, filter);
			break;
		}
	}

	protected override void PostEvaluate () {
		Straight.PostEvaluateStraight (ref results, ref bucketSpades, ref specialSpades);
		Straight.PostEvaluateStraight (ref results, ref bucketHearts, ref specialHearts);
		Straight.PostEvaluateStraight (ref results, ref bucketClubs, ref specialClubs);
		Straight.PostEvaluateStraight (ref results, ref specialDiamonds, ref specialDiamonds);

		results.Sort ();
	}
}