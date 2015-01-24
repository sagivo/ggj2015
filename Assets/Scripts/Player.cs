using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Player : MonoBehaviour {
	float speed = 20;
	Vector3 v;
	Networking n;
	bool record = true; 
	List<string> records;
	int recordIndex = 0;
	float startReplay;

	void Start () {
		records = new List<string>();
		n = GetComponent<Networking>();
		n.OnGetComplete += (d) => {
			Debug.Log(d);
		};
	}
	
	void Update() {
		if (record) {
			v = Vector3.zero;
			if (Input.GetKeyDown(KeyCode.DownArrow)) { v = -Vector2.up; records.Add(Time.time.ToString() + ":" + "d"); }
			else if (Input.GetKeyDown(KeyCode.UpArrow)) { v = Vector2.up; records.Add(Time.time.ToString() + ":" + "u"); }
			else if (Input.GetKeyDown(KeyCode.RightArrow)) { v = Vector2.right; records.Add(Time.time.ToString() + ":" + "r"); }
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) { v = -Vector2.right; records.Add(Time.time.ToString() + ":" + "l"); }
			if (v!=Vector3.zero) {
				//var a = n.GET("/");
				transform.position += v * speed * Time.deltaTime;

			}
			if (v == Vector3.up) {
				var s = getRecords();
				Debug.Log(s);
				records = setRecords(s);
				Debug.Log(record);
				record = false;
			}
		} else { //replay
			if (startReplay==0) {
				startReplay = Time.time;
				transform.position = Vector3.zero;
			}
			if (recordIndex < records.Count && float.Parse(records[recordIndex].Split(':')[0]) + startReplay >= Time.time ){
				Vector3 v = new Vector3();
				string direction = records[recordIndex].Split(':')[1];
				if (direction == "u") v = Vector3.up;
				else if (direction == "d") v = Vector3.down;
				else if (direction == "l") v = Vector3.left;
				else if (direction == "r") v = Vector3.right;
				transform.position += v * speed * Time.deltaTime;
				recordIndex++;
			}
		}
	}

	string getRecords(){
		string s = "";
		foreach (var v in records) {
			s+= v;
		}
		//if (s.Length>1) s = s.Substring(0,s.Length-2);
		return s;
	}

	List<string> setRecords(string s){
		return new List<string>(s.Split(','));
	}
}