using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class GooglePlayServicesManager : MonoBehaviour, RealTimeMultiplayerListener
{

    bool IsConnectedToGoogleServices;
    public Text loginInfo;
    public GameObject signInButton;
    public GameObject signOutButton;
    public Button leaderboardButton;
    public Button archievementButton;

    public Button multiplayerButton;

    public uint MinOpponents = 1, MaxOpponents = 1;
    public Text feedbackText;
    public Text messagesReceived;

    public GameObject MenuCanvas;
    public GameObject MultiplayerCanvas;
    public GameObject MultiplayerObjects;

    MultiplayerController mc;

    bool connectingToRoom;

    public GameObject multiplayerFindPlayerPanel;
    public GameObject leaveSearchButton;
    public GameObject multiplayerStandardPanel;

    void Start()
    {
        mc = this.GetComponent<MultiplayerController>();
        connectingToRoom = false;

        var config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);

        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            UpdateLeaderboards();
            UpdateAchievements();
            loginInfo.text = "Signed In: " + PlayGamesPlatform.Instance.localUser.userName;
            signInButton.SetActive(false);
            signOutButton.SetActive(true);
            leaderboardButton.interactable = true;
            archievementButton.interactable = true;
            multiplayerButton.interactable = true;
        }
        else
        {
            loginInfo.text = "Not Signed In";
            signInButton.SetActive(true);
            signOutButton.SetActive(false);
            leaderboardButton.interactable = false;
            archievementButton.interactable = false;
            multiplayerButton.interactable = false;

            //Do not try to sign in when entering the menu. Player must click the sign in button to login.
            //SignIn();
        }
    }

    void StartMultiplayerGame()
    {
        MultiplayerCanvas.SetActive(true);
        MultiplayerObjects.SetActive(true);
        MenuCanvas.SetActive(false);
        mc.gameStarted = true;
    }

    void LeaveMultiplayerGame()
    {
        MultiplayerCanvas.SetActive(false);
        MultiplayerObjects.SetActive(false);

        multiplayerFindPlayerPanel.SetActive(true);
        leaveSearchButton.SetActive(false);

        MenuCanvas.SetActive(true);
        mc.Start();
    }

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
    }

    public void SignOut()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.SignOut();

            loginInfo.text = "Signed Out";

            signInButton.SetActive(true);
            signOutButton.SetActive(false);
            leaderboardButton.interactable = false;
            archievementButton.interactable = false;
            multiplayerButton.interactable = false;
        }
    }

    private void SignInCallback(bool success)
    {
        if (success)
        {
            loginInfo.text = "Signed In: " + PlayGamesPlatform.Instance.localUser.userName;
            UpdateLeaderboards();
            UpdateAchievements();
            signInButton.SetActive(false);
            signOutButton.SetActive(true);
            multiplayerButton.interactable = true;
            leaderboardButton.interactable = true;
            archievementButton.interactable = true;
        }
        else
        {
            signInButton.SetActive(true);
            signOutButton.SetActive(false);
            multiplayerButton.interactable = false;
            leaderboardButton.interactable = false;
            archievementButton.interactable = false;
            loginInfo.text = "Sign-in failed.";
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
    }

    public void IncrementAchievement(string id, int stepsToIncrement)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { });
    }

    public void ShowAchievementsUI()
    {
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    public void AddScoreToLeaderboard(string leaderboardId, long score)
    {
        PlayGamesPlatform.Instance.ReportScore(score, leaderboardId, success => { });
    }

    public void ShowLeaderboardsUI()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }

    public void CreateQuickGame(int gameVariant)
    {
        if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count != 0)
        {
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        }
        else
        {
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(MinOpponents, MaxOpponents,
                    (uint)gameVariant, this);

            mc.multiplayerMode = gameVariant;
            connectingToRoom = true;

            leaveSearchButton.SetActive(true);
            multiplayerStandardPanel.SetActive(false);
            multiplayerFindPlayerPanel.SetActive(false);
        }
    }

    public void OnRoomSetupProgress(float percent)
    {
        //feedbackText.text = "Connecting to game ...";
    }

    public void OnRoomConnected(bool success)
    {
        connectingToRoom = false;
        if (success)
        {
            //feedbackText.text = "Connected to room";
            StartMultiplayerGame();
        }
        else
        {
            //feedbackText.text = "Could not connect to a room. Please check if you are connected to google play services.";
        }
    }

    public void OnLeftRoom()
    {
        throw new NotImplementedException();
    }

    public void OnParticipantLeft(Participant participant)
    {
        throw new NotImplementedException();
    }

    public void OnPeersConnected(string[] participantIds)
    {
        throw new NotImplementedException();
    }

    public void OnPeersDisconnected(string[] participantIds)
    {
        throw new NotImplementedException();
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        string thisMessage = Encoding.ASCII.GetString(data);
        string[] raw = thisMessage.Split(new string[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);

        if (raw[0] == "T")
        {
            mc.tapCountOther = int.Parse(raw[1]);
            //messagesReceived.text = "Recieved message Taps!!!";
        }
        else if (raw[0] == "CD")
        {
            mc.currentDurationOther = float.Parse(raw[1]);
            //messagesReceived.text = "Recieved message countdownTime!!!";
        }
        else if (thisMessage == "L")
        {
            mc.gameOver = true;
            mc.iWin = true;
        }
    }

    void UpdateAchievements()
    {
        if (PlayerPrefs.GetInt("Best15") >= 50) { UnlockAchievement(TapTapRevolutionResources.achievement_the_50_in_15); }
        if (PlayerPrefs.GetInt("Best15") >= 100) { UnlockAchievement(TapTapRevolutionResources.achievement_the_100_in_15); }
        if (PlayerPrefs.GetInt("Best15") >= 150) { UnlockAchievement(TapTapRevolutionResources.achievement_the_150_in_15); }
        if (PlayerPrefs.GetInt("Best15") >= 200) { UnlockAchievement(TapTapRevolutionResources.achievement_the_200_in_15); }
        if (PlayerPrefs.GetInt("Best15") >= 250) { UnlockAchievement(TapTapRevolutionResources.achievement_you_are_15__awesome); }

        if (PlayerPrefs.GetInt("Best30") >= 100) { UnlockAchievement(TapTapRevolutionResources.achievement_the_100_in_30); }
        if (PlayerPrefs.GetInt("Best30") >= 200) { UnlockAchievement(TapTapRevolutionResources.achievement_the_200_in_30); }
        if (PlayerPrefs.GetInt("Best30") >= 300) { UnlockAchievement(TapTapRevolutionResources.achievement_the_300_in_30); }
        if (PlayerPrefs.GetInt("Best30") >= 400) { UnlockAchievement(TapTapRevolutionResources.achievement_the_400_in_30); }
        if (PlayerPrefs.GetInt("Best30") >= 500) { UnlockAchievement(TapTapRevolutionResources.achievement_you_are_30_awesome); }

        if (PlayerPrefs.GetInt("Best60") >= 200) { UnlockAchievement(TapTapRevolutionResources.achievement_the_200_in_60); }
        if (PlayerPrefs.GetInt("Best60") >= 400) { UnlockAchievement(TapTapRevolutionResources.achievement_the_400_in_60); }
        if (PlayerPrefs.GetInt("Best60") >= 600) { UnlockAchievement(TapTapRevolutionResources.achievement_the_600_in_60); }
        if (PlayerPrefs.GetInt("Best60") >= 800) { UnlockAchievement(TapTapRevolutionResources.achievement_the_800_in_60); }
        if (PlayerPrefs.GetInt("Best60") >= 1000) { UnlockAchievement(TapTapRevolutionResources.achievement_you_are_60_awesome); }
    }

    public void LeaveGame()
    {
        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        LeaveMultiplayerGame();
    }

    public void SendMyMessage(string message, bool reliable)
    {
        //messagesReceived.text = "Sending message: " + message;
        byte[] myMessage = Encoding.ASCII.GetBytes(message);
        //bool reliable = true;
        string participantId = mc.playerOther.ParticipantId;

        PlayGamesPlatform.Instance.RealTime.SendMessage(reliable, participantId, myMessage);
    }


    public void LeaveSearch()
    {
        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        multiplayerFindPlayerPanel.SetActive(true);
        leaveSearchButton.SetActive(false);

        connectingToRoom = false;
    }

    public void Update()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count == 0)
            {
                if (connectingToRoom)
                {
                    feedbackText.text = "Connecting To Server ...";
                }
                else
                {
                    feedbackText.text = "Not connected to game server ...";
                }
            }
            else if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count == 1)
            {
                feedbackText.text = "Finding Match ...";
            }
            else if (MenuCanvas.activeSelf && PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count == 2)
            {
                feedbackText.text = "Match Found";
            }
        }
    }
}