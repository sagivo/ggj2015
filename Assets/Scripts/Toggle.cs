using UnityEngine;
using System.Collections;

public class Toggle : MonoBehaviour {
	public Sprite sprite1;
	public Sprite sprite2;
	SpriteRenderer s;
	// Use this for initialization
	void Start () {
		s = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	public void toggle () {
		s.sprite = (s.sprite == sprite1) ? sprite2 : sprite1;
	}
}
