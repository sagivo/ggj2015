using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void toggle(){
		gameObject.SetActive(!gameObject.activeSelf);
	}
}
