using UnityEngine;
using System.Collections;
using System.Xml;

using System.Collections.Generic;
using Facebook.MiniJSON;

public class SplashController : MonoBehaviour {

    public GameObject PrefabShiftScene;
    private GameObject m_ShiftScene;

    private void CreateNewSettingsFile(string path)
    {
        XmlTextWriter writer = new XmlTextWriter(path, System.Text.Encoding.UTF8);
        writer.WriteStartDocument(true);
        writer.Formatting = Formatting.Indented;
        writer.Indentation = 2;
        writer.WriteStartElement("Settings");
            writer.WriteStartElement("Sound");
            writer.WriteAttributeString("enable", "true");
            writer.WriteEndElement();

            writer.WriteStartElement("KeySound");
            writer.WriteAttributeString("type", "piano");
            writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Close();
        Debug.Log("Done creating new settings.xml");
    }

    private void GetSettings()
    {
        string path = Application.persistentDataPath + "/settings.xml";
        XmlDocument doc = new XmlDocument();
        try
        {
            doc.Load(path);
        }
        catch
        {
            Debug.Log("Settings not found!");
            CreateNewSettingsFile(path);
            doc.Load(path);
        }

        // Set global configuration
        if (doc.ChildNodes[1].ChildNodes[0].Attributes["enable"].Value == "true")
            PlayerPrefs.SetInt("Settings_Sound", 1);
        else PlayerPrefs.SetInt("Settings_Sound", 0);
    }

    IEnumerator GetAdsConfig()
    {
        string url = "http://5play.mobi:8888/adsservice-0.0.1-SNAPSHOT/getads/config";
        string str = "{\"os\":\""
                      + "android"
                      + "\",\"did\":\""
                      + PlayerPrefs.GetString("Device ID")
                      + "\",\"appid\":\"7\"}";
        var encoding = new System.Text.UTF8Encoding();
        Dictionary<string, string> dict;
        dict = new Dictionary<string, string>();
        dict.Add("Content-Type", "application/json");
        dict.Add("Content-Length", str.Length.ToString());
        WWW request = new WWW(url, encoding.GetBytes(str), dict);

        yield return request;
        Debug.Log(request.text);
        if (request.text.Length != 0)
        {
            // Success
            Debug.Log("Gotcha!");
            Dictionary<string, object> resultDict = Json.Deserialize(request.text) as Dictionary<string, object>;
            Dictionary<string, object> config = resultDict["config"] as Dictionary<string, object>;
            Debug.Log("banner: " + config["admodID_banner"]);
            Debug.Log("popup: " + config["admodID_popup"]);

            // AdsService.SetAdmobInfo((string)config["admodID_banner"], (string)config["admodID_popup"], 2, 1, 3);
            // Set info
            PlayerPrefs.SetString("AdmobBannerId", (string)config["admodID_banner"]);
            PlayerPrefs.SetString("AdmobPopupId", (string)config["admodID_popup"]);
            PlayerPrefs.SetString("5playBannerLink", (string)resultDict["img_banner"]);
            PlayerPrefs.SetString("5playPopupLink", (string)resultDict["img_vertical"]);

            PlayerPrefs.SetInt("AdmobBannerTimes", 2);
            PlayerPrefs.SetInt("AdmobPopupTimes", 1);
            PlayerPrefs.SetInt("AdmobNoPopupTimes", 3);

            PlayerPrefs.SetInt("AdmobBannerTimesShown", 0);
            PlayerPrefs.SetInt("AdmobPopupTimesShown", 0);
            PlayerPrefs.SetInt("NoPopupTimesShown", 0);
        }
        else
        {
            // Failed
            Debug.Log("Failed");
            // AdsService.SetAdmobInfo("", "");
        }
        StartCoroutine(StartGameRoutine());
    }

    IEnumerator StartGameRoutine()
    {
        yield return new WaitForSeconds(0.3f);
        Application.LoadLevelAdditive("MenuScene");
        m_ShiftScene.GetComponent<ShiftScene>().DoShiftOut("");
    }

	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 60;

        m_ShiftScene = Instantiate(PrefabShiftScene) as GameObject;
        GetSettings();
        StartCoroutine(GetAdsConfig());
	}
	
	// Update is called once per frame
	void Update () {
	}
}
