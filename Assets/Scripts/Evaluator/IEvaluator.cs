using UnityEngine;
using System;
using System.Collections.Generic;

public class IEvaluator<T> where T : class, new() {
	// This singleton doens't protect derived class from being intantiated using new, but at least it's bring convenience
	private static T sInstance = new T();
	public static T Instance { 
		get { 
			return sInstance; 
		} 
	}

	// Evaluator variables
	#pragma warning disable 0414
	protected CardSet cardSet = null;
	protected Func<Card, bool> filter = null;
	#pragma warning restore 0414

	// Result of operation
	protected List<PokerHand> results = new List<PokerHand>();
	public List<PokerHand> Results {
		get {
			if (cardSet != null) {
				Debug.LogWarning("Get Result before Evaluation Finish");
			}
			return results;
		}
	}

	// Initialize evaluator variable
	// cards need to be already sorted before evaluating
	// cards : collection of card to be evaluated
	// filterFunction : lambda function to evaluate condition of result
	public void Begin(CardSet cards, Func<Card, bool> filterFunction = null) {
		results.Clear ();
		cardSet = cards;
		filter = filterFunction != null ? filterFunction : any => true;

		if (cardSet == null)
			Debug.LogException(new Exception("CardSet is null"));
		if (cardSet.Count == 0)
			Debug.LogException (new Exception("Cannot evaluate empty cardset"));

		PreEvaluate ();
	}

	// Internal helper function, called only once before loop start
	protected virtual void PreEvaluate() {}

	// Evaluate it card in the cardset
	// index : index of current evaluated card
	public virtual void Evaluate(int index) {
		if (cardSet == null) {
			Debug.LogException(new Exception("Evaluate is Called before Begin()"));
		}
	}

	// Internal helper function, called only once after loop finish
	protected virtual void PostEvaluate() {}

	// End evaluating
	public void End() {
		PostEvaluate ();

		cardSet = null;
		filter = null;
	}

	// Perform Lazy evaluation
	// cards : collection of card to be evaluated
	// all : false to return only first occurence
	// filterFunction : lambda function to evaluate condition of result
	public List<PokerHand> LazyEvaluator(CardSet cards, bool all = false, Func<Card, bool> filterFunction = null) {
		Begin (cards, filterFunction);
		for (int i = 0; i < cards.Count; ++i) {
			Evaluate(i);
		}
		End ();
		return Results;
	}

	public virtual bool IsValid(CardSet cards, bool isSorted = false) { 
		return cards.Count > 0;
	}
}