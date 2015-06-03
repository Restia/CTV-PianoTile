using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GoogleMobileAds.Api;
using GoogleMobileAds;
using Facebook.MiniJSON;

// Gameobject for ads, just drag and drop
public class AdsServiceGO : MonoBehaviour {

    // Prefabs to generate 5play banner
    public GameObject PrefabCustomBanner;
    public GameObject PrefabCustomPopup;

    // Ads banners of Google admob and 5play
    private BannerView m_AdmobBanner;
    private GameObject m_CustomBanner;

    // Ads popup (intersititial) of Google admob and 5play
    private InterstitialAd m_Popup;
    private GameObject m_CustomPopup;

    // information for generating Ads
    private string m_AdmobBannerId = "";
    private string m_AdmobPopupId = "";
    private bool m_Enable = false;
    private int m_AdmobBannerTimes = 0;
    private int m_AdmobBannerTimesShown = 0;
    private int m_AdmobPopupTimes = 0;
    private int m_AdmobPopupTimesShown = 0;
    private int m_NoPopupTimes = 0;
    private int m_NoPopupTimesShown = 0;
    private string m_CustomBannerLink;
    private string m_CustomPopupLink;
    private string m_BannerClickUrl;
    private string m_PopupClickUrl;

    // Get Ads info had got from starting game
    private void GetInfo()
    {
        m_AdmobBannerId = PlayerPrefs.GetString("AdmobBannerId");
        m_AdmobPopupId = PlayerPrefs.GetString("AdmobPopupId");
        m_CustomBannerLink = PlayerPrefs.GetString("5playBannerLink");
        m_CustomPopupLink = PlayerPrefs.GetString("5playPopupLink");
        m_BannerClickUrl = PlayerPrefs.GetString("5playBannerClickUrl");
        m_PopupClickUrl = PlayerPrefs.GetString("5playPopupClickUrl");
        Debug.Log(m_CustomBannerLink);
        Debug.Log(m_BannerClickUrl);
        Debug.Log(m_CustomPopupLink);
        Debug.Log(m_PopupClickUrl);

        if (m_AdmobBannerId.Length == 0 || m_AdmobPopupId.Length == 0)
            return;
        else
        {
            m_Enable = true;
            m_AdmobBannerTimes = PlayerPrefs.GetInt("AdmobBannerTimes");
            m_AdmobPopupTimes = PlayerPrefs.GetInt("AdmobPopupTimes");
            m_NoPopupTimes = PlayerPrefs.GetInt("AdmobNoPopupTimes");
            m_AdmobBannerTimesShown = PlayerPrefs.GetInt("AdmobBannerTimesShown");
            m_AdmobPopupTimesShown = PlayerPrefs.GetInt("AdmobPopupTimesShown");
            m_NoPopupTimesShown = PlayerPrefs.GetInt("NoPopupTimesShown");
        }
    }

    void OnDestroy()
    {
        if (m_Popup != null)
            m_Popup.Destroy();
        if (m_AdmobBanner != null)
            m_AdmobBanner.Destroy();
        if (m_CustomBanner != null)
            UnityEngine.Object.Destroy(m_CustomBanner);
        if (m_CustomPopup != null)
            UnityEngine.Object.Destroy(m_CustomPopup);
    }

	// Use this for initialization
	void Start () {
        GetInfo();
        RequestBanner();
        RequestPopup();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Get5PlayBannerAds()
    {
        string url = "http://5play.mobi:8888/adsservice-0.0.1-SNAPSHOT/getads/5play";
        string str = "{\"os\":\""
                      + "android"
                      + "\",\"did\":\""
                      + PlayerPrefs.GetString("Device ID")
                      + "\",\"appid\":\"7\",\"event\":\"banner\"}";

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
            Dictionary<string, object> resultDict = Json.Deserialize(request.text) as Dictionary<string, object>;

            PlayerPrefs.SetString("5playBannerLink", (string)resultDict["img_banner"]);
            PlayerPrefs.SetString("5playBannerClickUrl", (string)resultDict["store_url"]);
        }
        else
        {
            // Failed
            Debug.Log("Failed");
        }
    }

    IEnumerator Get5PlayPopupAds()
    {
        string url = "http://5play.mobi:8888/adsservice-0.0.1-SNAPSHOT/getads/5play";
        string str = "{\"os\":\""
                      + "android"
                      + "\",\"did\":\""
                      + PlayerPrefs.GetString("Device ID")
                      + "\",\"appid\":\"7\",\"event\":\"popup\"}";

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
            Dictionary<string, object> resultDict = Json.Deserialize(request.text) as Dictionary<string, object>;

            PlayerPrefs.SetString("5playPopupLink", (string)resultDict["img_vertical"]);
            PlayerPrefs.SetString("5playPopupClickUrl", (string)resultDict["store_url"]);
        }
        else
        {
            // Failed
            Debug.Log("Failed");
        }
    }

    public void RequestBanner()
    {
        if (!m_Enable)
            return;
        if (m_AdmobBannerTimesShown == m_AdmobBannerTimes)
        {
            // 5play ads
            Debug.Log("5play banner");
            GameObject obj = Instantiate(PrefabCustomBanner) as GameObject;
            obj.GetComponent<CustomBanner>().SetImageLink(m_CustomBannerLink, m_BannerClickUrl);
            StartCoroutine(Get5PlayBannerAds());
            obj.transform.parent = gameObject.transform;
            m_AdmobBannerTimesShown = 0;
            PlayerPrefs.SetInt("AdmobBannerTimesShown", 0);
        }
        else
        {
            // Admob ads
            m_AdmobBannerTimesShown++;
            PlayerPrefs.SetInt("AdmobBannerTimesShown", m_AdmobBannerTimesShown);
#if UNITY_EDITOR
            string adUnitId = "unused";
#elif UNITY_ANDROID
                string adUnitId = m_AdmobBannerId;
#elif UNITY_IPHONE
                string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
#else
                string adUnitId = "unexpected_platform";
#endif

            // Create a 320x50 banner at the top of the screen.
            m_AdmobBanner = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
            // Register for ad events.
            m_AdmobBanner.AdLoaded += HandleAdLoaded;
            m_AdmobBanner.AdFailedToLoad += HandleAdFailedToLoad;
            m_AdmobBanner.AdOpened += HandleAdOpened;
            m_AdmobBanner.AdClosing += HandleAdClosing;
            m_AdmobBanner.AdClosed += HandleAdClosed;
            m_AdmobBanner.AdLeftApplication += HandleAdLeftApplication;
            // Load a banner ad.
            m_AdmobBanner.LoadAd(createAdRequest());
        }
    }

    public void RequestPopup()
    {
        if (!m_Enable)
            return;
        if (m_NoPopupTimesShown < m_NoPopupTimes)
        {
            m_NoPopupTimesShown++;
            PlayerPrefs.SetInt("NoPopupTimesShown", m_NoPopupTimesShown);
            return;
        }
        else
        {
            m_NoPopupTimesShown = 0;
            PlayerPrefs.SetInt("NoPopupTimesShown", m_NoPopupTimesShown);
            if (m_AdmobPopupTimesShown == m_AdmobPopupTimes)
            {
                // 5play ads
                Debug.Log("5play popup");
                GameObject obj = Instantiate(PrefabCustomPopup) as GameObject;
                obj.GetComponent<CustomPopup>().SetImageLink(m_CustomPopupLink, m_PopupClickUrl);
                obj.transform.parent = gameObject.transform;
                StartCoroutine(Get5PlayPopupAds());
                m_AdmobPopupTimesShown = 0;
                PlayerPrefs.SetInt("AdmobPopupTimesShown", m_AdmobPopupTimesShown);
            }
            else
            {
                m_AdmobPopupTimesShown++;
                PlayerPrefs.SetInt("AdmobPopupTimesShown", m_AdmobPopupTimesShown);
#if UNITY_EDITOR
                string adUnitId = "unused";
#elif UNITY_ANDROID
            string adUnitId = m_AdmobPopupId;
#elif UNITY_IPHONE
            string adUnitId = "INSERT_IOS_INTERSTITIAL_AD_UNIT_ID_HERE";
#else
            string adUnitId = "unexpected_platform";
#endif

                // Create an interstitial.
                m_Popup = new InterstitialAd(adUnitId);
                // Register for ad events.
                m_Popup.AdLoaded += HandleInterstitialLoaded;
                m_Popup.AdFailedToLoad += HandleInterstitialFailedToLoad;
                m_Popup.AdOpened += HandleInterstitialOpened;
                m_Popup.AdClosing += HandleInterstitialClosing;
                m_Popup.AdClosed += HandleInterstitialClosed;
                m_Popup.AdLeftApplication += HandleInterstitialLeftApplication;
                // Load an interstitial ad.
                m_Popup.LoadAd(createAdRequest());
            }
        }
    }

    private AdRequest createAdRequest()
    {
        return new AdRequest.Builder()
                .AddTestDevice(AdRequest.TestDeviceSimulator)
                .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
                .AddKeyword("game")
                .SetGender(Gender.Male)
                .SetBirthday(new DateTime(1985, 1, 1))
                .TagForChildDirectedTreatment(false)
                .AddExtra("color_bg", "9B30FF")
                .Build();
    }

    #region Banner callback handlers

    private void HandleAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleAdLoaded event received.");
    }

    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    private void HandleAdOpened(object sender, EventArgs args)
    {
        Debug.Log("HandleAdOpened event received");
    }

    private void HandleAdClosing(object sender, EventArgs args)
    {
        Debug.Log("HandleAdClosing event received");
    }

    private void HandleAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleAdClosed event received");
    }

    private void HandleAdLeftApplication(object sender, EventArgs args)
    {
        Debug.Log("HandleAdLeftApplication event received");
    }
    #endregion

    #region Interstitial callback handlers

    private void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialLoaded event received.");
        m_Popup.Show();
    }

    private void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("HandleInterstitialFailedToLoad event received with message: " + args.Message);
    }

    private void HandleInterstitialOpened(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialOpened event received");
    }

    private void HandleInterstitialClosing(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialClosing event received");
    }

    private void HandleInterstitialClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialClosed event received");
        m_Popup.Destroy();
    }

    private void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialLeftApplication event received");
    }

    #endregion
}
