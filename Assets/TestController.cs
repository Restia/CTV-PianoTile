﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Facebook;
using Facebook.MiniJSON;

public class TestController : MonoBehaviour {

    public static TestController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }

    public GameObject BoardFooter;

    public GameObject PrefabShiftScene;
    private GameObject m_ShiftScene;

    public GameObject PrefabFacebookDlg;

    public GameObject PrefabLeaderboard;
    private GameObject[] m_Leaderboards;
    private int[] m_Rank;
    private string m_AvatarLink;

    private string m_Url = "http://5play.mobi:8888/leader_board_record-1.0/v0.3/score";

    private float m_SlideDist;
    private int m_CurrBoardId;
    public bool IsSliding = false;
    public bool IsRepositioning = false;

    #region Board Sliding Control

    IEnumerator SlideLeftRoutine(int toBoardId)
    {
        IsSliding = true;
        Vector3 pos = m_Leaderboards[m_CurrBoardId].transform.position;
        Vector3 dest = new Vector3(pos.x - m_SlideDist, pos.y, pos.z);
        float startTime = Time.time;
        m_Leaderboards[toBoardId].SetActive(true);
        m_Leaderboards[toBoardId].transform.position = new Vector3(m_SlideDist,
            m_Leaderboards[toBoardId].transform.position.y,
            m_Leaderboards[toBoardId].transform.position.z);
        while (m_Leaderboards[m_CurrBoardId].transform.position != dest)
        {
            Vector3 newPos = Vector3.Lerp(pos,
                dest, (Time.time - startTime) / 0.2f);
            Vector3 tslVct = newPos - m_Leaderboards[m_CurrBoardId].transform.position;
            m_Leaderboards[m_CurrBoardId].transform.position += tslVct;
            m_Leaderboards[toBoardId].transform.position += tslVct;
            yield return null;
        }
        m_Leaderboards[m_CurrBoardId].SetActive(false);
        m_CurrBoardId = toBoardId;
        Debug.Log("Done slide left");
        IsSliding = false;
        yield return null;
    }

    IEnumerator SlideRightRoutine(int toBoardId)
    {
        IsSliding = true;
        Vector3 pos = m_Leaderboards[m_CurrBoardId].transform.position;
        Vector3 dest = new Vector3(pos.x + m_SlideDist, pos.y, pos.z);
        float startTime = Time.time;
        m_Leaderboards[toBoardId].SetActive(true);
        m_Leaderboards[toBoardId].transform.position = new Vector3(-m_SlideDist,
            m_Leaderboards[toBoardId].transform.position.y,
            m_Leaderboards[toBoardId].transform.position.z);
        while (m_Leaderboards[m_CurrBoardId].transform.position != dest)
        {
            Vector3 newPos = Vector3.Lerp(pos,
                dest, (Time.time - startTime) / 0.2f);
            Vector3 tslVct = newPos - m_Leaderboards[m_CurrBoardId].transform.position;
            m_Leaderboards[m_CurrBoardId].transform.position += tslVct;
            m_Leaderboards[toBoardId].transform.position += tslVct;
            yield return null;
        }
        m_Leaderboards[m_CurrBoardId].SetActive(false);
        m_CurrBoardId = toBoardId;
        Debug.Log("Done slide right");
        IsSliding = false;
        yield return null;
    }

    public void SlideLeft(int toBoardId)
    {
        StartCoroutine(SlideLeftRoutine(toBoardId));
        BoardFooter.GetComponent<Footer>().SetRank(m_Rank[toBoardId]);
    }

    public void SlideRight(int toBoardId)
    {
        StartCoroutine(SlideRightRoutine(toBoardId));
        BoardFooter.GetComponent<Footer>().SetRank(m_Rank[toBoardId]);
    }

    #endregion

    void OnDestroy()
    {
        if (m_Leaderboards != null)
            foreach (GameObject obj in m_Leaderboards)
                Object.Destroy(obj);
    }

    private void OnInitComplete()
    {
        Debug.Log("Done Facebook Init.");
        if (!FB.IsLoggedIn)
            StartCoroutine(FacebookLoginDlgRoutine());
        else
        {
            Debug.Log("Already logged in.");
            FB.API("/" + FB.UserId, HttpMethod.GET, OnInfoGot);
        };
    }

    #region Score Getters

    /* Be invoked when done logging in */
    private void LoginCallback(FBResult result)
    {
        Debug.Log("Done login.");
        FB.API("/" + FB.UserId, HttpMethod.GET, OnInfoGot);
    }

    /* Callback method for getting user info after logging in */
    private void OnInfoGot(FBResult result)
    {
        Dictionary<string, object> dict = Json.Deserialize(result.Text) as Dictionary<string, object>;

        Debug.Log((string)dict["first_name"]);
        PlayerPrefs.SetString("Facebook First Name", (string)dict["first_name"]);
        Debug.Log((string)dict["gender"]);
        PlayerPrefs.SetString("Facebook Gender", (string)dict["gender"]);
        Debug.Log((string)dict["last_name"]);
        PlayerPrefs.SetString("Facebook Last Name", (string)dict["last_name"]);
        Debug.Log((string)dict["link"]);
        PlayerPrefs.SetString("Facebook Link", (string)dict["link"]);
        Debug.Log((string)dict["name"]);
        PlayerPrefs.SetString("Facebook Name", (string)dict["name"]);

        Debug.Log(FB.UserId);
        PlayerPrefs.SetString("Facebook ID", FB.UserId);
        Debug.Log(SystemInfo.deviceUniqueIdentifier);
        PlayerPrefs.SetString("Device ID", SystemInfo.deviceUniqueIdentifier);
        Debug.Log(SystemInfo.operatingSystem);
        PlayerPrefs.SetString("Operating System", SystemInfo.operatingSystem);

        // Do stuffs
        StartCoroutine(UpdateScore());
    }

    IEnumerator PostScoreRoutine(int highScore)
    {
        // Post to recent score.
        string deviceId = PlayerPrefs.GetString("Deviced ID");
        string facebookId = PlayerPrefs.GetString("Facebook ID");
        string score = highScore.ToString();
        string postString = "{\"appid\":\"whitetiles\",\"boardname\":\""
            + "Arcade Recent"
            + "\",\"did\":\""
            + deviceId
            + "\",\"expired\":\"86400000\",\"fid\":\""
            + facebookId
            + "\",\"order\":\"descending\",\"os\":\""
            + PlayerPrefs.GetString("Operating System")
            + "\",\"score\":\""
            + score
            + "\"}";

        string postString2 = "{\"appid\":\"whitetiles\",\"boardname\":\""
            + "Arcade Top"
            + "\",\"did\":\""
            + deviceId
            + "\",\"expired\":\"86400000\",\"fid\":\""
            + facebookId
            + "\",\"order\":\"descending\",\"os\":\""
            + PlayerPrefs.GetString("Operating System")
            + "\",\"score\":\""
            + score
            + "\"}";

        var encoding = new System.Text.UTF8Encoding();

        // Post to Arcade Recent
        Dictionary<string, string> dict;
        dict = new Dictionary<string, string>();
        dict.Add("Content-Type", "application/json");
        dict.Add("Content-Length", postString.Length.ToString());
        WWW request = new WWW(m_Url, encoding.GetBytes(postString), dict);

        yield return request;
        Debug.Log(request.text);

        // Post to Arcade Top
        dict = new Dictionary<string, string>();
        dict.Add("Content-Type", "application/json");
        dict.Add("Content-Length", postString2.Length.ToString());
        request = new WWW(m_Url, encoding.GetBytes(postString2), dict);

        yield return request;
        Debug.Log(request.text);
    }

    IEnumerator UpdateScore()
    {
        // Step 1: Sign in to server
        yield return StartCoroutine(SignInRoutine());
        // Step 2: Check if the score is posted to server
        //      + If it was posted: proceed to Step 3
        //      + If it was not posted: post to server
        string path = Application.persistentDataPath;
        XmlDocument xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.Load(path + "/highscore.xml");
        }
        catch
        {
            Debug.Log("Not found " + path);
            CreateHighscoreFile(path);
            xmlDoc.Load(path + "/highscore.xml");
        }
        int highScore;
        int.TryParse(xmlDoc.ChildNodes[1].ChildNodes[1].InnerText, out highScore);
        if (xmlDoc.ChildNodes[1].ChildNodes[1].Attributes["posted"].Value == "false")
        {
            yield return StartCoroutine(PostScoreRoutine(highScore));
            xmlDoc.ChildNodes[1].ChildNodes[1].Attributes["posted"].Value = "true";
            xmlDoc.Save(path + "/highscore.xml");
        }
        // Step 3: Get 2 tables of score back
        m_Leaderboards = new GameObject[4];
        m_Rank = new int[4];

        yield return StartCoroutine(GetScoreRoutine("Arcade Recent", 0));
        yield return StartCoroutine(GetScoreRoutine("Arcade Top", 1));

        // ID = 2 => TopFriend
        m_Leaderboards[2] = Instantiate(PrefabLeaderboard) as GameObject;
        for (int i = 0; i < 10; i++)
            m_Leaderboards[2].GetComponent<ItemContainer>().AddItem("", "test name2", 10.23f);
        m_Leaderboards[2].SetActive(false);
        m_Rank[2] = -1;

        // ID = 3 => Classic All-Time
        yield return StartCoroutine(GetScoreRoutine("Classic Top", 3));

        m_Leaderboards[0].SetActive(true);
        BoardFooter.GetComponent<Footer>().SetRank(m_Rank[0]);
    }

    private void CreateHighscoreFile(string path)
    {
        XmlTextWriter writer = new XmlTextWriter(path + "/highscore.xml", System.Text.Encoding.UTF8);
        writer.WriteStartDocument(true);
        writer.Formatting = Formatting.Indented;
        writer.Indentation = 2;
        writer.WriteStartElement("Highscore");
        writer.WriteStartElement("Classic");
        writer.WriteAttributeString("posted", "false");
        writer.WriteString("0");
        writer.WriteEndElement();
        writer.WriteStartElement("Arcade");
        writer.WriteAttributeString("posted", "false");
        writer.WriteString("0");
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Close();
    }

    IEnumerator SignInRoutine()
    {
        string url = "http://5play.mobi:8888/leader_board_record-1.0/v0.3/account/facebook/signin";
        string str = "{\"appid\":\"whitetiles\",\"did\":\""
                     + PlayerPrefs.GetString("Device ID")
                     + "\",\"fb_basic_info\":{\"first_name\":\""
                     + PlayerPrefs.GetString("Facebook First Name")
                     + "\",\"gender\":\""
                     + PlayerPrefs.GetString("Facebook Gender")
                     + "\",\"id\":\""
                     + PlayerPrefs.GetString("Facebook ID")
                     + "\",\"last_name\":\""
                     + PlayerPrefs.GetString("Facebook Last Name")
                     + "\",\"link\":\""
                     + PlayerPrefs.GetString("Facebook Link")
                     + "\",\"name\":\""
                     + PlayerPrefs.GetString("Facebook Name")
                     + "\"},\"os\":\""
                     + PlayerPrefs.GetString("Operating System")
                     + "\"}";
        Debug.Log(str);
        var encoding = new System.Text.UTF8Encoding();
        Dictionary<string, string> dict;
        dict = new Dictionary<string, string>();
        dict.Add("Content-Type", "application/json");
        dict.Add("Content-Length", encoding.GetBytes(str).Length.ToString());
        WWW request = new WWW(url, encoding.GetBytes(str), dict);
        yield return request;

        Debug.Log(request.text);
    }

    IEnumerator GetScoreRoutine(string boardName, int toBoardId)
    {
        string url = "http://5play.mobi:8888/leader_board_record-1.0/v0.3/score/topscore";
        string str = "{\"end\":\"15\",\"os\":\""
                      + PlayerPrefs.GetString("Operating System")
                      + "\",\"did\":\""
                      + PlayerPrefs.GetString("Device ID")
                      + "\",\"order\":\"descending\",\"boardname\":\""
                      + boardName
                      + "\",\"fid\":\""
                      + PlayerPrefs.GetString("Facebook ID")
                      + "\",\"appid\":\"whitetiles\",\"start\":\"0\"}";

        var encoding = new System.Text.UTF8Encoding();
        Dictionary<string, string> dict;
        dict = new Dictionary<string, string>();
        dict.Add("Content-Type", "application/json");
        dict.Add("Content-Length", str.Length.ToString());
        WWW request = new WWW(url, encoding.GetBytes(str), dict);

        yield return request;
        Debug.Log(request.text);

        m_Leaderboards[toBoardId] = Instantiate(PrefabLeaderboard) as GameObject;
        Dictionary<string, object> resultDict = Json.Deserialize(request.text) as Dictionary<string, object>;
        Dictionary<string, object> aboutMe = resultDict["me"] as Dictionary<string, object>;
        if (aboutMe != null)
        {
            int.TryParse(aboutMe["rank"].ToString(), out m_Rank[toBoardId]);
            m_AvatarLink = "https://graph.facebook.com/" + aboutMe["fid"] + "/picture?width=150&height=150";
            BoardFooter.GetComponent<Footer>().SetAvatarLink(m_AvatarLink);
        }
        else m_Rank[toBoardId] = -1;
        // m_AvatarLink = "https://graph.facebook.com/" + aboutMe["fid"] + "/picture?width=150&height=150";

        List<object> list = (List<object>)resultDict["users"];
        foreach (object obj in list)
        {
            Dictionary<string, object> user = obj as Dictionary<string, object>;
            string avatar_url = "https://graph.facebook.com/" + user["fid"] + "/picture?width=150&height=150";
            string name = (string)user["name"];
            string scoreStr = user["score"].ToString();
            float score;
            float.TryParse(scoreStr, out score);
            m_Leaderboards[toBoardId].GetComponent<ItemContainer>().AddItem(avatar_url, name, score);
        }
        m_Leaderboards[toBoardId].SetActive(false);
        yield return null;
    }

    #endregion

    private void Yes_Clicked()
    {
        Debug.Log("Yes");
        FB.Login("email,user_friends,public_profile", LoginCallback);
    }

    private void No_Clicked()
    {
        Debug.Log("No");
        Application.LoadLevelAdditive("MenuScene");
        m_ShiftScene.GetComponent<ShiftScene>().DoShiftOut("");
    }

    IEnumerator FacebookLoginDlgRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        GameObject obj = Instantiate(PrefabFacebookDlg) as GameObject;
        obj.GetComponent<DialogController>().GetYesButton().GetComponent<DlgButtonBg>().EvtClicked += Yes_Clicked;
        obj.GetComponent<DialogController>().GetNoButton().GetComponent<DlgButtonBg>().EvtClicked += No_Clicked;
        yield return null;
    }

	// Use this for initialization
	void Start () {
        m_SlideDist = Camera.main.orthographicSize * Camera.main.aspect * 2.0f;
        m_CurrBoardId = 0;
        if (!FB.IsInitialized)
            FB.Init(OnInitComplete);
        m_ShiftScene = Instantiate(PrefabShiftScene) as GameObject;
        m_ShiftScene.GetComponent<ShiftScene>().ShiftInWhenReady = true;
	}

    public void BackToMainMenu()
    {
        StopAllCoroutines();
        if (m_Leaderboards != null)
        {
            foreach (GameObject obj in m_Leaderboards)
                obj.GetComponent<MonoBehaviour>().StopAllCoroutines();
        }
        Application.LoadLevelAdditive("MenuScene");
        m_ShiftScene.GetComponent<ShiftScene>().DoShiftOut("");
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMainMenu();
        }
	}
}
