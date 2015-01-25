using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public GameObject player2Prefub;
	public Text text;
	int lvl = 1;
	int[] lvlTime = new int[]{0,20,7,15,35};
	//string player2Id;
	List<string> player2Records;
	Vector3 startPos1;
	Vector3 startPos2;
	string playerId;
	string gameId; 
	float recordTime = 0;
	float speed = 10;
	Vector3 v;
	Networking n;
	List<string> records;
	int recordIndex1 = 0;
	int recordIndex2 = 0;
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
			recordTime += Time.deltaTime;
			if (Input.GetKeyDown(KeyCode.DownArrow)) { v = -Vector2.up; records.Add( recordTime.ToString() + ":" + "d"); }
			else if (Input.GetKeyDown(KeyCode.UpArrow)) { v = Vector2.up; records.Add(recordTime.ToString() + ":" + "u"); }
			else if (Input.GetKeyDown(KeyCode.RightArrow)) { v = Vector2.right; records.Add(recordTime.ToString() + ":" + "r"); }
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) { v = -Vector2.right; records.Add(recordTime.ToString() + ":" + "l"); }
			if (v!=Vector3.zero) {
				//player1.transform.Translate(v * speed * Time.deltaTime);
				player1.transform.rigidbody2D.velocity = v*speed;
				//transform.position += v * speed * Time.deltaTime;
			}
			if (Input.GetKeyDown(KeyCode.Space)) {
				//finishLvl();
			}

		} else if (gameMode == gameModeType.Replay) { //replay
			recordTime+=Time.deltaTime;
			if (recordIndex1 < records.Count && float.Parse(records[recordIndex1].Split(':')[0]) <= recordTime ){
				var v = vectorForKey(records[recordIndex1].Split(':')[1]);
				if (v!=Vector3.zero) //player1.transform.Translate(v * speed * Time.deltaTime);
					player1.transform.rigidbody2D.velocity = v*speed;
				recordIndex1++;
			}
			//move player2 
			if (recordIndex2 < player2Records.Count && float.Parse(player2Records[recordIndex2].Split(':')[0]) <= recordTime ){
				var v = vectorForKey(player2Records[recordIndex2].Split(':')[1]);
				if (v!=Vector3.zero) //player2.transform.Translate(v * speed * Time.deltaTime);
					player2.transform.rigidbody2D.velocity = v*speed;
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

	void updateCounter(){
		int time = int.Parse(text.text);
		if (time <= 0) { CancelInvoke("updateCounter"); finishLvl(); }
		else text.text = (time-1).ToString();
	}

	void checkForPlayerActions(){
		n.GET("/actions/" + gameId);
	}

	void finishLvl(){
		initBoard();
		string actions = getRecords();
		n.POST("/actions/"+playerId, "actions", actions);
		gameMode = gameModeType.PostingActions;
		text.text = "Waiting for player2";
		//lvl++;
	}

	void initBoard(){
		player1.transform.position = startPos1; 
		player2.transform.position = startPos2;
		player1.transform.rotation = player2.transform.rotation = Quaternion.identity;
		player1.transform.rigidbody2D.velocity = player2.transform.rigidbody2D.velocity = Vector2.zero;
	}

	public void startRecording(){
		initBoard();

		recordTime = 0;
		text.text = lvlTime[lvl].ToString();
		InvokeRepeating("updateCounter", 1 ,1);
		gameMode = gameModeType.Record;
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
				startPos1 = player1.transform.position;
				startPos2 = player2.transform.position;
				player1.GetComponent<SpriteRenderer>().color = Color.blue;
				player2.GetComponent<SpriteRenderer>().color = Color.red;
				startRecording();
			}
			break;
		case gameModeType.PostingActions:
			if (d == "ok"){
				InvokeRepeating("checkForPlayerActions",0,2);
				gameMode = gameModeType.waitingForPartnerActions;
			}
			break;
		case gameModeType.waitingForPartnerActions:
			if (d!="wait"){
				CancelInvoke("checkForPlayerActions");
				var data = d.Split('|');
				records = recordsFromString( (data[0] == playerId) ? data[1] : data[3] );
				player2Records = recordsFromString( (data[0] != playerId) ? data[1] : data[3] );

				text.text = "Replay";
				recordTime = 0;
				gameMode = gameModeType.Replay;
				//Invoke("startRecording", lvlTime[lvl]);
			}
			break;		
		default: break;
		}
	}
} 