using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {
	bool platerNear;
	public Laser[] lasers;
	Toggle t;
	// Use this for initialization
	void Start () {
		t = GetComponent<Toggle>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space) && platerNear){
			t.toggle();
			foreach (var l in lasers) {
				l.toggle();
			}
		}
	}

	void OnCollisionEnter2D(Collision2D o) {
		platerNear = true;
	}

	void OnCollisionExit2D(Collision2D o) {
		platerNear = false;
	}

	public void trigger(){
		if (!platerNear) return;
		t.toggle();
		foreach (var l in lasers) {
			l.toggle();
		}
	}
}
