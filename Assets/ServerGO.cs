using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerGO : MonoBehaviour {

    // Request cached
    private Dictionary<string, string> m_LastRequest;
    private string m_LastTextReturn;
    private string m_LastURL;

    // Callback function
    public delegate void DoneRequestCallback();
    private DoneRequestCallback m_Callback;

    IEnumerator SendRequestRoutine()
    {
        string requestStr = "{";
        foreach (KeyValuePair<string, string> entry in m_LastRequest)
        {
            requestStr += ("\"" + entry.Key + "\":\"" + entry.Value + "\",");
        }
        requestStr = requestStr.Remove(requestStr.Length - 1, 1);
        requestStr += "}";

        Debug.Log(requestStr);

        var encoding = new System.Text.UTF8Encoding();
        Dictionary<string, string> requestDict;
        requestDict = new Dictionary<string, string>();
        requestDict.Add("Content-Type", "application/json");
        requestDict.Add("Content-Length", requestStr.Length.ToString());
        WWW request = new WWW(m_LastURL, encoding.GetBytes(requestStr), requestDict);

        yield return request;
        m_LastTextReturn = request.text;

        if (m_Callback != null)
            m_Callback();
    }

    public void SendRequest(Dictionary<string, string> requestCnt, string url, DoneRequestCallback callback = null)
    {
        m_LastURL = url;
        m_LastRequest = requestCnt;
        m_Callback = callback;
        StartCoroutine(SendRequestRoutine());
    }

    private void TestCallback()
    {
        Debug.Log(m_LastTextReturn);
    }

	// Use this for initialization
	void Start () {
        Dictionary<string, string> testRequestCnt = new Dictionary<string, string>();
        string url = "http://5play.mobi:8888/leader_board_record-1.0/v0.3/score/topscore";
        testRequestCnt.Add("end", "15");
        testRequestCnt.Add("os", "android");
        testRequestCnt.Add("did", SystemInfo.deviceUniqueIdentifier);
        testRequestCnt.Add("order", "descending");
        testRequestCnt.Add("boardname", "Arcade Recent");
        testRequestCnt.Add("fid", "-1");
        testRequestCnt.Add("appid", "whitetiles");
        testRequestCnt.Add("start", "0");
        SendRequest(testRequestCnt, url, TestCallback);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
