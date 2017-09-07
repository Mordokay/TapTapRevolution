using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoogleNetworkManager : MonoBehaviour, RealTimeMultiplayerListener
{
    public uint MinOpponents = 1, MaxOpponents = 1;
    public Text feedbackText;

    uint gameMode;
    float extraValueConnect;

    bool wantsToConnect;
    int gameVariantForConnection;

    private void Start()
    {
        wantsToConnect = false;
        InvokeRepeating("SendShit", 0.0f, 2.0f);
    }

    public void CreateQuickGame(int gameVariant)
    {
        if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count == 0)
        {
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(MinOpponents, MaxOpponents,
                (uint)gameVariant, this);

            wantsToConnect = false;
            PlayerPrefs.SetInt("MultiplayerMode", gameVariant);
        }
        else
        {
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();
            wantsToConnect = true;
            gameVariantForConnection = gameVariant;
        }
        //PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI();
    }
    
    public static Participant GetOpponent(IRealTimeMultiplayerClient multiplayerClient)
    {
        foreach (Participant p in multiplayerClient.GetConnectedParticipants())
        {
            if (!p.ParticipantId.Equals(multiplayerClient.GetSelf().ParticipantId))
            {
                return p;
            }
        }
        return null;
    }

    public static string GetOpponentName(IRealTimeMultiplayerClient multiplayerClient)
    {
        Participant p = GetOpponent(multiplayerClient);
        return p == null ? "(anonymous)" : p.DisplayName;
    }
    
    public void OnRoomSetupProgress(float percent)
    {
        feedbackText.text = "Room creation progress: " + percent;
    }

    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            feedbackText.text = "Connected to room" ;
        }
        else
        {
            feedbackText.text = "Could not connect to a room. Please check if you are connected to google play services.";
        }
    }

    public void OnLeftRoom()
    { 
        if (wantsToConnect)
        {
            CreateQuickGame(gameVariantForConnection);
        }
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
        if (data[0] == (byte)'I')
        {
            feedbackText.text = "Recieved " + data[1].ToString() + " from: " + senderId;
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
        if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count == 2)
        {
            //feedbackText.text = "Send Shit  " + Time.timeSinceLevelLoad + " <> " +  PlayGamesPlatform.Instance.RealTime.GetSelf(); ;
            string participantId = "";
            
            foreach(Participant p in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
            {
                if (!p.ParticipantId.Equals(PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId))
                {
                    participantId = p.ParticipantId;
                }
            }

            byte[] mPosPacket = new byte[2];
            mPosPacket[0] = (byte)'I';
            mPosPacket[1] = (byte)(UnityEngine.Random.Range(0, 10));
            //PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, mPosPacket);
            //PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, mPosPacket);
            PlayGamesPlatform.Instance.RealTime.SendMessage(true, participantId, mPosPacket);
            PlayGamesPlatform.Instance.RealTime.SendMessage(false, participantId, mPosPacket);
        }
    }

    private void Update()
    {
        
        if(PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count == 2)
        {
            //SendShit();
            SceneManager.LoadScene(2);
        }
        
        else if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count > 0)
        {
            feedbackText.text = "Connected Players: " + PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count.ToString();
            foreach(Participant p in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
            {
                feedbackText.text += System.Environment.NewLine + p.DisplayName;
            }
        }
        else
        {
            //feedbackText.text = "No players connected to any room";
        }
        
    }
}