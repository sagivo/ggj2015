using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Networking : MonoBehaviour {

	public System.Action<string> OnGetComplete;
	public System.Action<string> OnPostComplete;

	string baseUrl = "https://young-brushlands-8741.herokuapp.com";
	//string baseUrl = "http://0.0.0.0:8080";

	void Start(){}
	
	public WWW GET(string url)
	{		
		WWW www = new WWW (baseUrl + url);
		StartCoroutine (WaitForRequest (www, false));
		return www; 
	}

	public WWW POST(string url, string key, string val){
		Dictionary<string,string> p = new Dictionary<string, string>();
		p[key] = val;
		return POST(url, p);
	}
	
	public WWW POST(string url, Dictionary<string,string> post)
	{
		WWWForm form = new WWWForm();
		if (post!=null)
			foreach(KeyValuePair<string,string> post_arg in post)
			{
				form.AddField(post_arg.Key, post_arg.Value);
			}
		else form.AddField("a","a"); //stam
		WWW www = new WWW(baseUrl + url, form);
		
		StartCoroutine(WaitForRequest(www, true));
		return www; 
	}
	
	private IEnumerator WaitForRequest(WWW www, bool post)
	{
		yield return www;
		Debug.Log(((post) ? "POST " : "GET ") +  www.url + ":" + www.text);
		if (www.error == null)
		{
			if (OnGetComplete!=null && !post) OnGetComplete(www.text);
			else if (OnPostComplete!=null && post) OnPostComplete(www.text);
		} else {
			Debug.Log("WWW Error for "+www.url+": "+ www.error);
		}    
	}
}


