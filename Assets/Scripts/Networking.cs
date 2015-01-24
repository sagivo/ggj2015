using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Networking : MonoBehaviour {

	public System.Action<string> OnGetComplete;
	public System.Action<string> OnPostComplete;

	string baseUrl = "http://www.google.com";

	void Start(){}
	
	public WWW GET(string url)
	{
		
		WWW www = new WWW (baseUrl + url);
		StartCoroutine (WaitForRequest (www));
		return www; 
	}
	
	public WWW POST(string url, Dictionary<string,string> post)
	{
		WWWForm form = new WWWForm();
		foreach(KeyValuePair<string,string> post_arg in post)
		{
			form.AddField(post_arg.Key, post_arg.Value);
		}
		WWW www = new WWW(url, form);
		
		StartCoroutine(WaitForRequest(www));
		return www; 
	}
	
	private IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		
		// check for errors
		if (www.error == null)
		{
			if (OnGetComplete!=null) OnGetComplete(www.text);

		} else {
			Debug.Log("WWW Error: "+ www.error);
		}    
	}
}


