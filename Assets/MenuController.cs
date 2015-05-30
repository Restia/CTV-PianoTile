using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using GoogleMobileAds.Api;
using GoogleMobileAds;
using Facebook.MiniJSON;

public class MenuController : MonoBehaviour {

    public static MenuController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }

    public GameObject PrefabShiftScene;
    private GameObject ShiftSceneObj;

    public GameObject MenuRating;
    public GameObject MenuLeaderboard;
    public GameObject MenuClassic;
    public GameObject MenuArcade;
    public GameObject MenuMoreGame;
    public GameObject MenuToggleSound;
    public GameObject MenuExit;
    public GameObject MenuChatting;

    private bool m_DisableFunction;
    private BannerView bannerView;
    private InterstitialAd interstitial;

    private void MenuArcade_Clicked()
    {
        if (!m_DisableFunction)
        {
            m_DisableFunction = true;
            ShiftSceneObj.GetComponent<ShiftScene>().DoShiftOut("");
            Application.LoadLevelAdditive("ArcadeScene");
        }
    }

    private void MenuClassic_Clicked()
    {
        if (!m_DisableFunction)
        {
            m_DisableFunction = true;
            ShiftSceneObj.GetComponent<ShiftScene>().DoShiftOut("");
            Application.LoadLevelAdditive("ClassicScene");
        }
    }

    private void MenuLeaderboard_Clicked()
    {
        if (!m_DisableFunction)
        {
            m_DisableFunction = true;
            ShiftSceneObj.GetComponent<ShiftScene>().DoShiftOut("");
            Application.LoadLevelAdditive("LeaderBoard1");
        }
    }

    private void MenuExit_Clicked()
    {
        if (!m_DisableFunction)
        {
            m_DisableFunction = true;
            Application.Quit();
        }
    }

    private void MenuSoundToggle_Clicked()
    {
        Debug.Log(MenuToggleSound.GetComponent<MainMenuToggleSound>().GetValue());
        string path = Application.persistentDataPath + "/settings.xml";
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        if (PlayerPrefs.GetInt("Settings_Sound") == 1)
            doc.ChildNodes[1].ChildNodes[0].Attributes["enable"].Value = "true";
        else doc.ChildNodes[1].ChildNodes[0].Attributes["enable"].Value = "false";
        doc.Save(path);
    }

    private void MoreGame_Clicked()
    {
    }

    private void Chatting_Clicked()
    {
        Debug.Log("interstitial");
        // AdsService.RequestInterstitial();
    }

    private void ShiftSceneCallback()
    {
        m_DisableFunction = false;
    }


	// Use this for initialization
	void Start () {
        MenuArcade.GetComponent<MainMenuItem>().EvtClicked += MenuArcade_Clicked;
        MenuClassic.GetComponent<MainMenuItem>().EvtClicked += MenuClassic_Clicked;
        MenuLeaderboard.GetComponent<MainMenuItem>().EvtClicked += MenuLeaderboard_Clicked;
        MenuExit.GetComponent<MainMenuItem>().EvtClicked += MenuExit_Clicked;
        MenuToggleSound.GetComponent<MainMenuToggleSound>().EvtClicked += MenuSoundToggle_Clicked;
        MenuMoreGame.GetComponent<MainMenuItem>().EvtClicked += MoreGame_Clicked;
        MenuChatting.GetComponent<MainMenuItem>().EvtClicked += Chatting_Clicked;
        m_DisableFunction = true;
        ShiftSceneObj = Instantiate(PrefabShiftScene) as GameObject;
        ShiftSceneObj.GetComponent<ShiftScene>().ShiftInWhenReady = true;
        ShiftSceneObj.GetComponent<ShiftScene>().ShiftInCallback += ShiftSceneCallback;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
