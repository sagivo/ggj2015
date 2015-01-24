using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Player : MonoBehaviour {
	public GameObject player2;
	Vector3 startPos;
	int id;
	int gameId; 
	float speed = 20;
	Vector3 v;
	Networking n;
	bool record = true; 
	List<string> records;
	int recordIndex = 0;
	float startReplay;
	enum gameModeType {waitingForPlaers};
	gameModeType gameMode;

	void Start () {
		gameMode = gameModeType.waitingForPlaers;
		startPos = transform.position;
		records = new List<string>();
		n = GetComponent<Networking>();
		n.OnGetComplete += (d) => {
			Debug.Log("GET:" + d);
			if (gameMode == gameModeType.waitingForPlaers){

			}
		};
		n.OnPostComplete += (d) => {
			Debug.Log("POST:" + d);
		};
	}
	
	void Update() {
		if (record) {
			v = startPos;
			if (Input.GetKeyDown(KeyCode.DownArrow)) { v = -Vector2.up; records.Add(Time.time.ToString() + ":" + "d"); }
			else if (Input.GetKeyDown(KeyCode.UpArrow)) { v = Vector2.up; records.Add(Time.time.ToString() + ":" + "u"); }
			else if (Input.GetKeyDown(KeyCode.RightArrow)) { v = Vector2.right; records.Add(Time.time.ToString() + ":" + "r"); }
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) { v = -Vector2.right; records.Add(Time.time.ToString() + ":" + "l"); }
			if (v!=Vector3.zero) {
				transform.Translate(v * speed * Time.deltaTime);
				//transform.position += v * speed * Time.deltaTime;

			}
			if (Input.GetKeyDown(KeyCode.Space)) {
				string s = getRecords();
				//n.POST("/games/322118074/actions/1", "actions", s);
				n.GET("/games/322118074/actions");
				records = setRecords(s);
				record = false;
			}

		} else { //replay
			if (startReplay==0) {
				startReplay = Time.time;
				transform.position = Vector3.zero;
			}
			if (recordIndex < records.Count && float.Parse(records[recordIndex].Split(':')[0]) + startReplay <= Time.time ){
				Vector3 v = new Vector3();
				string direction = records[recordIndex].Split(':')[1];
				if (direction == "u") v = Vector3.up;
				else if (direction == "d") v = Vector3.down;
				else if (direction == "l") v = Vector3.left;
				else if (direction == "r") v = Vector3.right;
				transform.Translate(v * speed * Time.deltaTime);
				recordIndex++;
			}
		}
	}

	string getRecords(){
		return string.Join(",",  records.ToArray());
	}

	List<string> setRecords(string s){
		return new List<string>(s.Split(','));
	}
}