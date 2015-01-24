using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Player : MonoBehaviour {
	public GameObject player2Prefub;
	//string player2Id;
	List<string> player2Records;
	Vector3 startPos;
	string playerId;
	string gameId; 
	float speed = 20;
	Vector3 v;
	Networking n;
	List<string> records;
	int recordIndex1 = 0;
	int recordIndex2 = 0;
	float startReplayTime;
	float startRecordTime;
	enum gameModeType {init, waitingForPartner, Record, PostingActions, waitingForPartnerActions, Replay};
	gameModeType gameMode;
	GameObject player1;
	GameObject player2;

	void Start () {
		records = new List<string>();
		gameMode = gameModeType.init;

		n = GetComponent<Networking>();
		n.POST("/init",null);

		n.OnGetComplete += (d) => {
			handleResponse(d);
		};
		n.OnPostComplete += (d) => {
			handleResponse(d);
		};
	}
	
	void Update() {
		if (gameMode == gameModeType.Record) {
			v = Vector3.zero;
			if (Input.GetKeyDown(KeyCode.DownArrow)) { v = -Vector2.up; records.Add((Time.time - startRecordTime ).ToString() + ":" + "d"); }
			else if (Input.GetKeyDown(KeyCode.UpArrow)) { v = Vector2.up; records.Add((Time.time - startRecordTime ).ToString() + ":" + "u"); }
			else if (Input.GetKeyDown(KeyCode.RightArrow)) { v = Vector2.right; records.Add((Time.time - startRecordTime ).ToString() + ":" + "r"); }
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) { v = -Vector2.right; records.Add((Time.time - startRecordTime ).ToString() + ":" + "l"); }
			if (v!=Vector3.zero) {
				player1.transform.Translate(v * speed * Time.deltaTime);
				//transform.position += v * speed * Time.deltaTime;
			}
			if (Input.GetKeyDown(KeyCode.Space)) {
				string actions = getRecords();
				n.POST("/actions/"+playerId, "actions", actions);
				gameMode = gameModeType.PostingActions;
			}

		} else if (gameMode == gameModeType.Replay) { //replay
			if (recordIndex1 < records.Count && float.Parse(records[recordIndex1].Split(':')[0]) + startReplayTime <= Time.time ){
				var v = vectorForKey(records[recordIndex1].Split(':')[1]);
				if (v!=Vector3.zero) player1.transform.Translate(v * speed * Time.deltaTime);
				recordIndex1++;
			}
			//move player2 
			if (recordIndex2 < player2Records.Count && float.Parse(player2Records[recordIndex2].Split(':')[0]) + startReplayTime <= Time.time ){
				var v = vectorForKey(player2Records[recordIndex2].Split(':')[1]);
				if (v!=Vector3.zero) player2.transform.Translate(v * speed * Time.deltaTime);
				recordIndex2++;
			}
		}
	}

	Vector3 vectorForKey(string direction){
		Vector3 v = Vector3.zero;
		if (direction == "u") v = Vector3.up;
		else if (direction == "d") v = Vector3.down;
		else if (direction == "l") v = Vector3.left;
		else if (direction == "r") v = Vector3.right;
		return v;
	}

	string getRecords(){
		return string.Join(",",  records.ToArray());
	}

	List<string> recordsFromString(string s){
		return new List<string>(s.Split(','));
	}

	void checkForPlayers(){
		n.GET("/check/" + playerId);
	}

	void checkForPlayerActions(){
		n.GET("/actions/" + gameId);
	}

	void handleResponse(string d){
		switch (gameMode) {
		case gameModeType.init:
			playerId = d;
			gameMode = gameModeType.waitingForPartner;
			InvokeRepeating("checkForPlayers",0,2);
			break;
		case gameModeType.waitingForPartner:
			if (d!="wait"){
				var data = d.Split('|');
				CancelInvoke("checkForPlayers");
				gameId = data[0];
				player1 = (data[1] == playerId) ? gameObject : player2Prefub;
				player2 = (data[1] != playerId) ? gameObject : player2Prefub;
				startPos = player1.transform.position;
				startRecordTime = Time.time;
				gameMode = gameModeType.Record;

			}
			break;
		case gameModeType.PostingActions:
			if (d == "ok"){
				InvokeRepeating("checkForPlayerActions",0,2);
				gameMode = gameModeType.waitingForPartnerActions;
				player1.transform.position = startPos;
			}
			break;
		case gameModeType.waitingForPartnerActions:
			if (d!="wait"){
				CancelInvoke("checkForPlayerActions");
				gameMode = gameModeType.Replay;
				var data = d.Split('|');
				records = recordsFromString( (data[0] == playerId) ? data[1] : data[3] );
				player2Records = recordsFromString( (data[0] != playerId) ? data[1] : data[3] );
				gameMode = gameModeType.Replay;
				startReplayTime = Time.time;
			}
			break;		
		default: break;
		}
		Debug.Log(gameMode);
	}
} 