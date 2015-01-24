using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	float speed = 20;
	Vector3 v;

	void Start () {
	}
	
	void Update() {
		v = Vector3.zero;
		if (Input.GetKeyDown(KeyCode.DownArrow)) v = -Vector2.up;
		else if (Input.GetKeyDown(KeyCode.UpArrow)) v = Vector2.up;
		else if (Input.GetKeyDown(KeyCode.RightArrow)) v = Vector2.right;
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) v = -Vector2.right;
		if (v!=Vector3.zero) 
			transform.position += v * speed * Time.deltaTime;
	}


}