using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour {
	float speed = 10;
	Vector2 v;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		v = Vector2.zero;
		if (Input.GetKeyDown(KeyCode.DownArrow)) v = -Vector2.up;
		else if (Input.GetKeyDown(KeyCode.UpArrow)) v = Vector2.up; 
		else if (Input.GetKeyDown(KeyCode.RightArrow)) v = Vector2.right; 
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) v = -Vector2.right;
		if (v!=Vector2.zero) {

			//transform.Translate(v * speed * Time.deltaTime);
			transform.rigidbody2D.velocity = v*speed;
			//rigidbody2D.AddForce (Vector3.forward * speed);
			//rigidbody2D.AddRelativeForce(v * speed);
		}
	}
}
