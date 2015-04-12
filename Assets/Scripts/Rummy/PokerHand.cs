using System;
using System.Collections;

public class PokerHand : IComparable<PokerHand> {
	public enum CombinationType {
		Invalid,
		Single,
		Pair,
		Triple,
		Straight,
		Flush,
		FullHouse,
		FourOfAKind,
		StraightFlush,
		RoyalFlush,
		Dragon
	}

	private Card key;
	private CardSet cards;
	private CombinationType combination;

	public PokerHand(CardSet cards, Card key, CombinationType combination) {
		this.cards = cards;
		this.key = key;
		this.combination = combination;
	}

	public Card Key {
		get { return key; }
	}

	public CardSet Cards {
		get { return cards; }
	}

	public CombinationType Combination {
		get { return combination; }
	}
	
	public static bool operator >(PokerHand lhs, PokerHand rhs) {
		return lhs.CompareTo(rhs) > 0;
	}
	
	public static bool operator <(PokerHand lhs, PokerHand rhs) {
		return lhs.CompareTo(rhs) < 0;
	}
	
	public int CompareTo(PokerHand other) {
		if (this.combination == other.combination)
			return this.key.CompareTo (other.key);
		else 
			return (int)this.combination - (int)other.combination;
	}
}