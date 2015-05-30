using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GoogleMobileAds.Api;
using GoogleMobileAds;

public class AdsService : MonoBehaviour
{
    private static BannerView m_AdmobBanner;
    private static GameObject m_CustomBanner;
    private static InterstitialAd m_Interstitial;
    
    private static string m_AdmobBannerId = "";
    private static string m_AdmobInterstitialId = "";
    private static bool m_Enable = false;
    private static int m_AdmobBannerTimes = 0;
    private static int m_AdmobBannerTimesShown = 0;
    private static int m_AdmobPopupTimes = 0;
    private static int m_AdmobPopupTimesShown = 0;
    private static int m_NoPopupTimes = 0;
    private static int m_NoPopupTimesShown = 0;

    // public static GameObject m_PrefabCustomBanner;
    
    public static void SetAdmobInfo(string bannerId, string interstitialId,
        int admobBannerTimes = 0, int admobPopupTimes = 0, int noPopupTimes = 0)
    {
        if (bannerId.Length == 0 && interstitialId.Length == 0)
        {
            return;
        }
        else
        {
            m_Enable = true;
            m_AdmobBannerId = bannerId;
            m_AdmobInterstitialId = interstitialId;
            m_AdmobBannerTimes = admobBannerTimes;
            m_AdmobPopupTimes = admobPopupTimes;
            m_NoPopupTimes = noPopupTimes;
            // m_PrefabCustomBanner = Resources.Load("CustomBanner") as GameObject;
        }
    }

    public static void DestroyAds()
    {
        if (m_Interstitial != null)
            m_Interstitial.Destroy();
        if (m_AdmobBanner != null)
            m_AdmobBanner.Destroy();
        if (m_CustomBanner != null)
            UnityEngine.Object.Destroy(m_CustomBanner);
    }

    // For now, for ANDROID devices only
    public static void RequestBanner()
    {
        if (!m_Enable)
            return;
        if (m_AdmobBannerTimesShown == m_AdmobBannerTimes)
        {
            // 5play ads
            Debug.Log("5play banner");
            m_AdmobBannerTimesShown = 0;
        }
        else
        {
            // Admob ads
            m_AdmobBannerTimesShown++;
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

    public static void RequestInterstitial()
    {
        if (!m_Enable)
            return;
        if (m_NoPopupTimesShown < m_NoPopupTimes)
        {
            m_NoPopupTimesShown++;
            return;
        }
        else
        {
            m_NoPopupTimesShown = 0;
            if (m_AdmobPopupTimesShown == m_AdmobPopupTimes)
            {
                // 5play ads
                Debug.Log("5play popup");
                m_AdmobPopupTimesShown = 0;
            }
            else
            {
                m_AdmobPopupTimesShown++;
#if UNITY_EDITOR
                string adUnitId = "unused";
#elif UNITY_ANDROID
            string adUnitId = m_AdmobInterstitialId;
#elif UNITY_IPHONE
            string adUnitId = "INSERT_IOS_INTERSTITIAL_AD_UNIT_ID_HERE";
#else
            string adUnitId = "unexpected_platform";
#endif

                // Create an interstitial.
                m_Interstitial = new InterstitialAd(adUnitId);
                // Register for ad events.
                m_Interstitial.AdLoaded += HandleInterstitialLoaded;
                m_Interstitial.AdFailedToLoad += HandleInterstitialFailedToLoad;
                m_Interstitial.AdOpened += HandleInterstitialOpened;
                m_Interstitial.AdClosing += HandleInterstitialClosing;
                m_Interstitial.AdClosed += HandleInterstitialClosed;
                m_Interstitial.AdLeftApplication += HandleInterstitialLeftApplication;
                // Load an interstitial ad.
                m_Interstitial.LoadAd(createAdRequest());
            }
        }
    }

    // Returns an ad request with custom ad targeting.
    // TODO (void): modify this
    private static AdRequest createAdRequest()
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

    private static void HandleAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleAdLoaded event received.");
    }

    private static void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    private static void HandleAdOpened(object sender, EventArgs args)
    {
        Debug.Log("HandleAdOpened event received");
    }

    private static void HandleAdClosing(object sender, EventArgs args)
    {
        Debug.Log("HandleAdClosing event received");
    }

    private static void HandleAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleAdClosed event received");
    }

    private static void HandleAdLeftApplication(object sender, EventArgs args)
    {
        Debug.Log("HandleAdLeftApplication event received");
    }

    #endregion

    #region Interstitial callback handlers

    private static void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialLoaded event received.");
        m_Interstitial.Show();
    }

    private static void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("HandleInterstitialFailedToLoad event received with message: " + args.Message);
    }

    private static void HandleInterstitialOpened(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialOpened event received");
    }

    private static void HandleInterstitialClosing(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialClosing event received");
    }

    private static void HandleInterstitialClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialClosed event received");
        m_Interstitial.Destroy();
    }

    private static void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialLeftApplication event received");
    }

    #endregion
}
