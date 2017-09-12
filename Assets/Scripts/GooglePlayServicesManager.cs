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

    public uint MinOpponents = 1, MaxOpponents = 1;
    public Text feedbackText;
    public Text messagesReceived;

    public GameObject MenuCanvas;
    public GameObject MultiplayerCanvas;
    public GameObject MultiplayerObjects;

    MultiplayerController mc;

    void Start()
    {
        mc = this.GetComponent<MultiplayerController>();

        var config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);

        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            UpdateLeaderboards();

            loginInfo.text = "Signed In: " + PlayGamesPlatform.Instance.localUser.userName;
            signInButton.SetActive(false);
            signOutButton.SetActive(true);
            leaderboardButton.interactable = true;
            archievementButton.interactable = true;
        }
        else
        {
            SignIn();
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
        }
    }

    private void SignInCallback(bool success)
    {
        if (success)
        {
            loginInfo.text = "Signed In: " + PlayGamesPlatform.Instance.localUser.userName;
            UpdateLeaderboards();
            signInButton.SetActive(false);
            signOutButton.SetActive(true);
            leaderboardButton.interactable = true;
            archievementButton.interactable = true;
        }
        else
        {
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
        }
    }

    public void OnRoomSetupProgress(float percent)
    {
        feedbackText.text = "Connecting to game ...";
    }

    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            feedbackText.text = "Connected to room";
            StartMultiplayerGame();

        }
        else
        {
            feedbackText.text = "Could not connect to a room. Please check if you are connected to google play services.";
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

        if(raw[0] == "T")
        {
            mc.tapCountOther = int.Parse(raw[1]);
            //messagesReceived.text = "Recieved message Taps!!!";
        }
        else if (raw[0] == "CD")
        {
            mc.currentDurationOther = float.Parse(raw[1]);
            //messagesReceived.text = "Recieved message countdownTime!!!";
        }
        else
        {
            //messagesReceived.text = "Recieved other shit!";
        }
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

    public void Update()
    {
        if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count < 2)
        {
            feedbackText.text = "Connected Players: " + PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count.ToString();
            foreach (Participant p in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
            {
                feedbackText.text += System.Environment.NewLine + p.DisplayName;
            }
        }
    }
}