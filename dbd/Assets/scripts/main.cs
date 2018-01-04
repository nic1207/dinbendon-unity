using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class main : MonoBehaviour {
	public GameObject PanelWelcome;
	public GameObject PanelLogin;
	public GameObject PanelMain;
	public GameObject[] Contents;

	private delegate void OnSuccess(string result);
	private delegate void OnError(string msg);

	private string _serviceUrl = "http://10.62.145.145/oService/";
	private string _savedAccount = string.Empty;
	private string _tmpAcc = string.Empty;
	// Use this for initialization
	void Start () {
		if (PanelLogin != null) {
			PanelLogin.SetActive (false);
		}
		if (PanelWelcome != null) {
			PanelWelcome.SetActive (true);
		}
		if (PanelMain != null) {
			PanelMain.SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void doShowLoginUI()
	{
		if (PanelLogin != null) {
			PanelLogin.SetActive (true);
		}
	}

	public void doLogin()
	{
		InputField[] ff = PanelLogin.GetComponentsInChildren<InputField> ();
		string acc = ff [0].text;
		string pass = ff [1].text;
		Debug.Log ("doLogin("+acc+","+pass+")");
		//if (PanelLogin != null) {
		//	PanelLogin.SetActive (false);
		//}
		//if (PanelMain != null) {
		//	PanelMain.SetActive (true);
		//}
		string url = _serviceUrl+ "Pass.asp?User=" + acc + "&Pwd=" + pass + "&Action=ChkPass";
		_tmpAcc = acc;
		Debug.Log ("url="+url);
		StartCoroutine (DoRequest (url, OnLoginSuccess, OnLoginError));
	}

	public void doOrder(int index)
	{
		Debug.Log ("doOrder("+index+")");
		string oname = string.Empty;
		string url = _serviceUrl + "Dinner/Order.asp?User=" + _savedAccount + "&Action=Order&DinnerType=1&PackName="+oname;
		Debug.Log ("url="+url);
		StartCoroutine (DoRequest (url, OnOrderSuccess, OnOrderError));
	}

	public void doGetMenu()
	{
		Debug.Log ("doGetMenu()");
		string url = _serviceUrl + "Dinner/Order.asp?User=" + _savedAccount;
		Debug.Log ("url="+url);
		StartCoroutine (DoRequest (url, OnGetMenuSuccess, OnGetMenuError));
	}

	public void doSetAlert(int index)
	{
		Debug.Log ("doSetAlert("+index+")");
	}

	public void doLogout()
	{
		Debug.Log ("doLogout()");
		if (PanelLogin != null) {
			PanelLogin.SetActive (false);
		}
		if (PanelMain != null) {
			PanelMain.SetActive (false);
		}
		if (PanelWelcome != null) {
			PanelWelcome.SetActive (true);
		}
	}

	public void doShowContentUI(int index)
	{
		if (Contents != null && Contents.Length > 0) {
			int i = 0;
			foreach(GameObject go in Contents) {
				if (i == index) {
					go.SetActive (true);
					if (index == 0) {
						doGetMenu ();
					}
				} else {
					go.SetActive (false);
				}
				i++;
			}
		}
	}

	IEnumerator DoRequest(string url, OnSuccess _onsuccess, OnError _onerror)
	{
		var downloader = new DownloadHandlerBuffer ();
		//var request = UnityWebRequest.Get(url);
		var request = new UnityWebRequest (url);
		request.downloadHandler = downloader;
		yield return request.Send ();

		if (request.isNetworkError) {
			Debug.LogError (request.error);
			if (_onerror != null)
				_onerror (request.error);
			yield break;
		}
		byte[] xx = downloader.data;
		//Debug.Log (downloader.data);
		var aa = Encoding.GetEncoding ("Big5").GetString (xx);
		//Debug.Log (aa);
		if (_onsuccess != null)
			_onsuccess (aa);
	}

	private void OnLoginSuccess(string result)
	{
		Debug.Log ("OnLoginSuccess("+result+")");
		if (result.IndexOf ("您尚未登錄基本資料") >= 0) {
			Debug.Log ("您尚未登錄基本資料");
		} else if (result.IndexOf ("密碼錯誤") >= 0) {
			Debug.Log ("密碼錯誤");
		} else {
			_savedAccount = _tmpAcc;
			Debug.Log ("登入成功");
			if (PanelLogin != null) {
				PanelLogin.SetActive (false);
			}
			if (PanelMain != null) {
				PanelMain.SetActive (true);
			}
		}
	}

	private void OnLoginError(string msg)
	{
		Debug.Log ("OnLoginError("+msg+")");
	}

	private void OnOrderSuccess(string result)
	{
		Debug.Log ("OnOrderSuccess("+result+")");
		if (result.IndexOf ("逾時請自理") >= 0) {
			Debug.Log ("逾時請自理");
		} else {
			Debug.Log ("訂餐成功");
		}
	}

	private void OnOrderError(string msg)
	{
		Debug.Log ("OnOrderError("+msg+")");
	}

	private void OnGetMenuSuccess(string result)
	{
		Debug.Log ("OnGetMenuSuccess("+result+")");
		if (result.IndexOf ("逾時請自理") >= 0) {
			Debug.Log ("逾時請自理");
		} else {
			int start = result.IndexOf ("<select name=\"PackName\">");
			if (start >= 0) {
				int end = result.IndexOf ("</select>");
				string tmp = result.Substring (start, end - start);
				Debug.Log (tmp);
				string op1 = string.Empty;
				int xs = result.IndexOf ("<option value=\"");

			}
		}
	}

	private void OnGetMenuError(string msg)
	{
		Debug.Log ("OnGetMenuError("+msg+")");
	}
}
