using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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

    public GameObject MenuCanvas;
    public GameObject MultiplayerCanvas;
    public GameObject MultiplayerObjects;

    void Start()
    {
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

        InvokeRepeating("SendShit", 0.0f, 2.0f);
    }

    void StartMultiplayerGame()
    {
        MultiplayerCanvas.SetActive(true);
        MultiplayerObjects.SetActive(true);
        MenuCanvas.SetActive(false);
    }

    void LeaveMultiplayerGame()
    {
        MultiplayerCanvas.SetActive(false);
        MultiplayerObjects.SetActive(false);
        MenuCanvas.SetActive(true);
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
        PlayGamesPlatform.Instance.RealTime.CreateQuickGame(MinOpponents, MaxOpponents,
                    (uint)gameVariant, this);
        PlayerPrefs.SetInt("MultiplayerMode", gameVariant);
        /*
        if (PlayGamesPlatform.Instance.RealTime.GetSelf().IsConnectedToRoom)
        {
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        }
        else
        {
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(MinOpponents, MaxOpponents,
                    (uint)gameVariant, this);
            PlayerPrefs.SetInt("MultiplayerMode", gameVariant);
        }
        */
    }

    public void OnRoomSetupProgress(float percent)
    {
        feedbackText.text = "Room creation progress: " + percent;
    }

    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            feedbackText.text = "Connected to room";
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

    public static Participant GetOpponent()
    {
        foreach (Participant p in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
        {
            if (!p.ParticipantId.Equals(PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId))
            {
                return p;
            }
        }
        return null;
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        if (data[0] == (byte)'I')
        {
            feedbackText.text +=  System.Environment.NewLine +  "Recieved " + data[1].ToString() + " from: " + senderId;
            foreach (Participant p in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
            {
                feedbackText.text += System.Environment.NewLine + p.DisplayName + " <> " + p.ParticipantId;
            }
        }
    }

    public void LeaveGame()
    {
        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
    }

    public void SendShit()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated && PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count == 2)
        {
            feedbackText.text = "Send Shit  " + Time.timeSinceLevelLoad + " <from> " +  PlayGamesPlatform.Instance.RealTime.GetSelf().DisplayName;
            
            string otherPlayerID = GetOpponent().ParticipantId;

            byte[] mPosPacket = new byte[2];
            mPosPacket[0] = (byte)'I';
            mPosPacket[1] = (byte)(UnityEngine.Random.Range(0, 10));

            PlayGamesPlatform.Instance.RealTime.SendMessage(true, otherPlayerID, mPosPacket); 
        }
    }

    public void Update()
    {
        /*
        if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count == 2)
        {
            //SendShit();
            //SceneManager.LoadScene(2);
        }

        else */
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