using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdsController : MonoBehaviour
{

    public RewardBasedVideoAd rewardBasedVideoAd;
    public Text addsText;

    public bool wantToLoadAd;

    public void Start()
    {
        wantToLoadAd = false;
        addsText.text = "";

        rewardBasedVideoAd = RewardBasedVideoAd.Instance;

        rewardBasedVideoAd.OnAdClosed += HandleOnAdClosed;
        rewardBasedVideoAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        rewardBasedVideoAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;
        rewardBasedVideoAd.OnAdLoaded += HandleOnAdLoaded;
        rewardBasedVideoAd.OnAdOpening += HandleOnAdOpening;
        rewardBasedVideoAd.OnAdRewarded += HandleOnAdRewarded;
        rewardBasedVideoAd.OnAdStarted += HandleOnAdStarted;

        InvokeRepeating("TryLoadingAd", 1.0f, 1.0f);
    }

    public void TryLoadingAd()
    {
        //addsText.text = "Trying To Load Ad" + System.Environment.NewLine + " ........." + UnityEngine.Random.Range(9, 99);
        if (!rewardBasedVideoAd.IsLoaded())
        {
            string adUnitId = "ca-app-pub-2936227452105377/2559504686";

#if UNITY_EDITOR
            adUnitId = "unused";
#endif

            AdRequest request = new AdRequest.Builder().Build();
            rewardBasedVideoAd.LoadAd(request, adUnitId);
        }
    }

    public void Update()
    {/*
        if (wantToLoadAd)
        {

            addsText.text = "Trying To Load Ad" + System.Environment.NewLine + " ........." + UnityEngine.Random.Range(9, 99);
            addsText.text += System.Environment.NewLine + "IsLoaded: " + rewardBasedVideoAd.IsLoaded().ToString();
            LoadRewardBasedAd();
            
            if (!rewardBasedVideoAd.IsLoaded())
            {
                LoadRewardBasedAd();
            }
            else
            {
                wantToLoadAd = false;
            }
            
        }
    */
    }

    public void ShowRewardBasedAd()
    {
        if (rewardBasedVideoAd.IsLoaded())
        {
            rewardBasedVideoAd.Show();
        }
    }

    public void requestLoadOfAd()
    {
        wantToLoadAd = true;
        //addsText.text = "Trying To Load Ad" + System.Environment.NewLine + " .........";
    }

    void LoadRewardBasedAd()
    {
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";

#if UNITY_EDITOR
        adUnitId = "unused";
#endif

        AdRequest request = new AdRequest.Builder().Build();
        rewardBasedVideoAd.LoadAd(request, adUnitId);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        //addsText.text += "Ads Loaded" + System.Environment.NewLine;
        wantToLoadAd = false;
    }
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //addsText.text = "Ads Failed To Load: " + args.Message + System.Environment.NewLine;
    }
    public void HandleOnAdOpening(object sender, EventArgs args)
    {
        //addsText.text += "Ads Opening" + System.Environment.NewLine;
    }
    public void HandleOnAdStarted(object sender, EventArgs args)
    {
        //addsText.text += "Ads Started" + System.Environment.NewLine;
    }
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        //addsText.text += "Ads Finished" + System.Environment.NewLine;
    }
    public void HandleOnAdRewarded(object sender, Reward args)
    {
        //Reward the user
        //addsText.text += String.Format("You just got {0} {1}!", args.Amount, args.Type) + System.Environment.NewLine;
    }
    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        //What to do with adds when you close the game? Not sure what can be done that matters ...
    }
}