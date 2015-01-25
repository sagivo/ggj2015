using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
	bool platerNear;
	public Laser[] lasers;
	Toggle t;

	void Start () {
		t = GetComponent<Toggle>();
	}
	
	// Update is called once per frame
	void Update () {
				
	}
	
	void OnTriggerEnter2D(Collider2D o) {
		t.toggle();
		foreach (var l in lasers) {
			l.toggle();
		}
	}
	
	void OnTriggerExit2D(Collider2D o) {
		t.toggle();
		foreach (var l in lasers) {
			l.toggle();
		}
	}

}
