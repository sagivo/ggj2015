using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
	List<string> records;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.DownArrow)) { v = -Vector2.up; records.Add((Time.time - startRecordTime ).ToString() + ":" + "d"); }
		else if (Input.GetKeyDown(KeyCode.UpArrow)) { v = Vector2.up; records.Add((Time.time - startRecordTime ).ToString() + ":" + "u"); }
		else if (Input.GetKeyDown(KeyCode.RightArrow)) { v = Vector2.right; records.Add((Time.time - startRecordTime ).ToString() + ":" + "r"); }
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) { v = -Vector2.right; records.Add((Time.time - startRecordTime ).ToString() + ":" + "l"); }
		if (v!=Vector3.zero) {
			transform.Translate(v * speed * Time.deltaTime);
		}
	}
}
