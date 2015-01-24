using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Player : MonoBehaviour {
	public GameObject player2;
	//string player2Id;
	List<string> player2Records;
	Vector3 startPos;
	string id;
	string gameId; 
	float speed = 20;
	Vector3 v;
	Networking n;
	List<string> records;
	int recordIndex = 0;
	float startReplay;
	enum gameModeType {init, waitingForPartner, Record, waitingForPartnerActions, Replay};
	gameModeType gameMode;

	void Start () {
		id = Random.Range(0,int.MaxValue).ToString();
		gameMode = gameModeType.init;
		startPos = transform.position;
		records = new List<string>();
		n = GetComponent<Networking>();

		n.OnGetComplete += (d) => {
			handleResponse(d);
		};
		n.OnPostComplete += (d) => {
			handleResponse(d);
		};
	}
	
	void Update() {
		if (gameMode == gameModeType.Record) {
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
				string actions = getRecords();
				n.POST("/games/"+gameId+"/actions/"+id, "actions", actions);
				InvokeRepeating("checkForActions",2,2);
				gameMode = gameModeType.waitingForPartnerActions;
			}

		} else if (gameMode == gameModeType.Replay) { //replay
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

	void setRecords(string s){
		records = new List<string>(s.Split(','));
	}

	void checkForPlayers(){
		n.GET("/check/" + id);
	}

	void checkForPlayerActions(){
		n.GET("/actions/" + gameId);
	}

	void handleResponse(string d){
		Debug.Log(gameMode);
		switch (gameMode) {
		case gameModeType.init:
			if (d!="ok") {
				gameMode = gameModeType.waitingForPartner;
				InvokeRepeating("checkForPlayers",0,2);
			}		
			break;
		case gameModeType.waitingForPartner:
			if (d!="wait"){
				CancelInvoke("checkForPlayers");
				gameId = d;
				gameMode = gameModeType.Record;
			}
			break;
		case gameModeType.waitingForPartnerActions:
			if (d!="wait"){
				CancelInvoke("checkForPlayerActions");
				gameId = d;
				gameMode = gameModeType.Replay;
				var data = d.Split('|');
				setRecords( (data[0] == id) ? data[1] : data[3] );
				gameMode = gameModeType.Replay;
				startReplay = Time.time;
				transform.position = Vector3.zero;
			}
			break;		
		default: break;
		}
	}
}