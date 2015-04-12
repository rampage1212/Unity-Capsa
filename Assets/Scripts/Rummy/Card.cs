using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour, IComparable<Card> {
	public enum Suit {
		Diamond,
		Club,
		Heart,
		Spade
	}
	public static string [] nominalOrder = {"3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A", "2" };

	public Suit suit;
	public Button button;
	int _score;
	string _nominal;

	PlayerController owner;
	Image image;

	public PlayerController Owner {
		get {
			if (owner == null)
				owner = GetComponentInParent<PlayerController> ();
			return owner;
		}
	}

	public string Nominal {
		set {
			_nominal = value.ToUpper();
			_score = Array.IndexOf<string>(nominalOrder, _nominal);
		}

		get {
			return _nominal;
		}
	}

	public int Score {
		get { return _score; }
	}

	void Awake () {
		button = GetComponent<Button> ();
		image = transform.GetChild(0).GetComponent<Image> ();
	}

	void Start() {
		image.sprite = SpriteCollection.From("playingCards").Get(suit.ToString() + "_" + _nominal);
	}

	public void ToggleSelect(){
		if (image.rectTransform.anchoredPosition == Vector2.zero) {
			image.rectTransform.anchoredPosition = new Vector2 (0, 20);
			Owner.View.OnCardMarked(this);
		} else {
			image.rectTransform.anchoredPosition = Vector2.zero;
			Owner.View.OnCardUnmarked(this);
		}
	}

	public void Select(bool flag){
		if (flag) {
			image.rectTransform.anchoredPosition = new Vector2 (0, 20);
			Owner.View.OnCardMarked(this);
		} else {
			image.rectTransform.anchoredPosition = Vector2.zero;
			Owner.View.OnCardUnmarked(this);
		} 
	}

	public static bool operator >(Card lhs, Card rhs){
		return lhs.CompareTo(rhs) > 0;
	}

	public static bool operator <(Card lhs, Card rhs){
		return lhs.CompareTo(rhs) < 0;
	}

	public int CompareTo(Card other) {
		if (this._score == other._score)
			return (int)this.suit - (int)other.suit;
		else 
			return this._score - other._score;
	}
}