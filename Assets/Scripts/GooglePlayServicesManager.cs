using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GooglePlayServicesManager : MonoBehaviour {

    bool IsConnectedToGoogleServices;
    public Text debugText;
    public GameObject signInButton;
    public GameObject signOutButton;
    public Button leaderboardButton;
    public Button archievementButton;

    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            SignInOut();
        }
        else
        {
            UpdateLeaderboards();

            debugText.text = "Signed In: " + PlayGamesPlatform.Instance.localUser.userName;
            signInButton.SetActive(false);
            signOutButton.SetActive(true);
            leaderboardButton.interactable = true;
            archievementButton.interactable = true;
        }
    }

    public void SignInOut()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            debugText.text = "Signing In ...";
            PlayGamesPlatform.Instance.Authenticate(SignInCallback);
            //Social.localUser.Authenticate(SignInCallback);
        }
        else
        {
            PlayGamesPlatform.Instance.SignOut();
            debugText.text = "Signed Out";

            signInButton.SetActive(true);
            signOutButton.SetActive(false);
            leaderboardButton.interactable = false;
            archievementButton.interactable = false;
        }
    }

    private void SignInCallback(bool success)
    {
        if (success)
        {
            debugText.text = "Signed In: " + PlayGamesPlatform.Instance.localUser.userName;
            UpdateLeaderboards();
            signInButton.SetActive(false);
            signOutButton.SetActive(true);
            leaderboardButton.interactable = true;
            archievementButton.interactable = true;
        }
        else
        {
            debugText.text = "Sign-in failed.";
        }
    }

    void UpdateLeaderboards()
    {
        AddScoreToLeaderboard(TapTapRevolutionResources.leaderboard_unlimited_mode, PlayerPrefs.GetInt("TapCount"));
        AddScoreToLeaderboard(TapTapRevolutionResources.leaderboard_new_15_seconds, PlayerPrefs.GetInt("Best15"));
        AddScoreToLeaderboard(TapTapRevolutionResources.leaderboard_30_seconds, PlayerPrefs.GetInt("Best30"));
        AddScoreToLeaderboard(TapTapRevolutionResources.leaderboard_60_seconds, PlayerPrefs.GetInt("Best60"));
    }

    public void UnlockAchievement(string id)
    {
        PlayGamesPlatform.Instance.ReportProgress(id, 100, success => { });
        //Social.ReportProgress(id, 100, success => { });
    }

    public void IncrementAchievement(string id, int stepsToIncrement)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { });
    }

    public void ShowAchievementsUI()
    {
        PlayGamesPlatform.Instance.ShowAchievementsUI();
        //Social.ShowAchievementsUI();
    }

    public void AddScoreToLeaderboard(string leaderboardId, long score)
    {
        PlayGamesPlatform.Instance.ReportScore(score, leaderboardId, success => { });
        //Social.ReportScore(score, leaderboardId, success => { });
    }

    public void ShowLeaderboardsUI()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
        //Social.ShowLeaderboardUI();
    }
}