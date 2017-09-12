using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Text;

public class MultiplayerController : MonoBehaviour
{
    public int multiplayerMode;
    public GameObject timeSlider;

    public float currentDurationMe;
    public float currentDurationOther;
    public int tapCountMe;
    public int tapCountOther;

    public Participant playerMe;
    public Participant playerOther;

    float startDuration;

    public bool gameOver;
    public bool iWin;

    public GameObject IWinPanel;
    public GameObject ILostPanel;

    public GameObject countdownPanel;
    public float countdownTimeMe;
    //public float countdownTimeOther;

    public Text messagesRecievedText;
    public Text gameModeText;
    public Text playerMeTaps;
    public Text playerOtherTaps;

    public bool gameStarted;
    public bool gameInitialized;
    GooglePlayServicesManager gpsm;
    public TextMesh tapScoreText;

    public void Start()
    {
        gpsm = this.GetComponent<GooglePlayServicesManager>();
        gameInitialized = false;

        countdownTimeMe = 5.0f;
        //countdownTimeOther = 5.0f;
        countdownPanel.SetActive(true);

        tapCountMe = 0;
        tapCountOther = 0;
        currentDurationMe = 999.0f;
        currentDurationOther = 999.0f;
        gameStarted = false;
        gameOver = false;
        iWin = false;

        IWinPanel.SetActive(false);
        ILostPanel.SetActive(false);
        timeSlider.GetComponent<Slider>().value = 1.0f;
        //messagesRecievedText.text = "No messages Received :(";
    }

    void checkWinLossStandard()
    {
        gameOver = true;

        //  I Win
        if (tapCountMe > tapCountOther)
        {
            iWin = true;
        }
        else //  I Lose
        {
            iWin = false;
        }
    }

    void CheckOtherPlayerLeft()
    {
        if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count == 1)
        {
            gameOver = true;
            iWin = true;
        }
    }

    public void IncrementMyTapCount()
    {
        tapCountMe++;
        //TODO: Send info to other player;

        gpsm.SendMyMessage("T:" + tapCountMe.ToString(), true);
        //SendMyMessage("tapCount:" + tapCountMe);
    }

    private void Update()
    {
        tapScoreText.text = "TAPS: " + tapCountMe;

        if (gameStarted)
        {
            //gpsm.SendMyMessage("CD:" + currentDurationMe.ToString());
            if (!gameInitialized)
            {
                gameInitialized = true;

                if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[0].DisplayName.Equals(PlayGamesPlatform.Instance.localUser.userName))
                {
                    playerMe = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[0];
                    playerOther = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[1];
                }
                else
                {
                    playerMe = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[1];
                    playerOther = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[0];
                }

                switch (multiplayerMode)
                {
                    case 0: //Standard Mode 15s
                        startDuration = 15.0f;
                        currentDurationMe = 15.0f;
                        gameModeText.text = "Game Mode: Standard 15";
                        timeSlider.GetComponentInChildren<Text>().text = startDuration + "s";
                        break;
                    case 1: //Standard Mode 30s
                        startDuration = 30.0f;
                        currentDurationMe = 30.0f;
                        gameModeText.text = "Game Mode: Standard 30";
                        timeSlider.GetComponentInChildren<Text>().text = startDuration + "s";
                        break;
                    case 2: //Standard Mode 60s
                        startDuration = 60.0f;
                        currentDurationMe = 60.0f;
                        gameModeText.text = "Game Mode: Standard 60";
                        timeSlider.GetComponentInChildren<Text>().text = startDuration + "s";
                        break;
                    case 3:  //Race Mode
                        timeSlider.SetActive(false);
                        gameModeText.text = "Game Mode: Race";
                        break;
                    case 4:  //Balance Mode
                        timeSlider.SetActive(false);
                        gameModeText.text = "Game Mode: Balance";
                        break;
                }

                //gameModeText.text = "Me: " + playerMe.DisplayName + "  PlayerOther:  " + playerOther.DisplayName + " " + Time.timeSinceLevelLoad;
            }

            if (gameOver)
            {
                if (iWin)
                {
                    IWinPanel.SetActive(true);
                    ILostPanel.SetActive(false);
                }
                else
                {
                    IWinPanel.SetActive(false);
                    ILostPanel.SetActive(true);
                }
            }
            else
            {
                CheckOtherPlayerLeft();

                playerMeTaps.text = playerMe.DisplayName + ": " + tapCountMe;
                playerOtherTaps.text = playerOther.DisplayName + ": " + tapCountOther;

                if (countdownTimeMe > 0.0f)
                {
                    countdownTimeMe -= Time.deltaTime;
                    countdownPanel.GetComponentInChildren<Text>().text = ((int)countdownTimeMe + 1).ToString();
                }
                else
                {
                    if (countdownPanel.activeSelf)
                    {
                        countdownPanel.SetActive(false);
                    }

                    //Standard Mode
                    if (multiplayerMode == 0 || multiplayerMode == 1 || multiplayerMode == 2)
                    {
                        if(currentDurationMe < 0.0f)
                        {
                            gpsm.SendMyMessage("CD:" + currentDurationMe.ToString(), true);
                            if (currentDurationOther < 0.0f)
                            {
                                checkWinLossStandard();
                            }
                        }
                        else
                        {
                            //messagesRecievedText.text = "currentDurationMe: " + currentDurationMe + " currentDurationOther: " + currentDurationOther;

                            currentDurationMe -= Time.deltaTime;

                            timeSlider.GetComponent<Slider>().value = currentDurationMe / startDuration;

                            timeSlider.GetComponentInChildren<Text>().text =
                                (int)(timeSlider.GetComponent<Slider>().value * startDuration) + "s";
                        }
                    }
                }
            }
        }
    }
}

/*
void Start() {
    InvokeRepeating("SendShit", 0.0f, 2.0f);

    messagesRecievedText.text = "No messages Received :(";

    InvokeRepeating("SendShit", 0.0f, 3.0f);
    SendShit();
    if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[0].DisplayName.Equals(PlayGamesPlatform.Instance.localUser.userName))
    {
        playerMe = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[0];
        playerOther = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[1];
    }
    else
    {
        playerMe = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[1];
        playerOther = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[0];
    }

    //SendMyMessage("hello my friend " + playerOther.DisplayName);
    //SendMyMessage("banana");

    multiplayerMode = PlayerPrefs.GetInt("MultiplayerMode");
    gameStarted = false;

    countdownTimeMe = 5.0f;
    countdownTimeOther = 5.0f;
    countdownPanel.SetActive(true);

    gameOver = false;
    iWin = false;

    debugText.text = "";

    switch (multiplayerMode)
    {
        case 0: //Standard Mode 15s
            startDuration = 15.0f;
            currentDurationMe = 15.0f;
            gameModeText.text = "Game Mode: Standard 15";
            break;
        case 1: //Standard Mode 30s
            startDuration = 30.0f;
            currentDurationMe = 30.0f;
            gameModeText.text = "Game Mode: Standard 30";
            break;
        case 2: //Standard Mode 60s
            startDuration = 60.0f;
            currentDurationMe = 60.0f;
            gameModeText.text = "Game Mode: Standard 60";
            break;
        case 3:  //Race Mode
            timeSlider.SetActive(false);
            gameModeText.text = "Game Mode: Race";
            break;
        case 4:  //Balance Mode
            timeSlider.SetActive(false);
            gameModeText.text = "Game Mode: Balance";
            break;
    }
}

public void SendShit()
{
gameModeText.text = "Sending Shit  " + Time.timeSinceLevelLoad;
string participantId = playerOther.ParticipantId;
byte[] mPosPacket = new byte[2];
mPosPacket[0] = (byte)'I';
mPosPacket[1] = (byte)'D';
PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, mPosPacket);
PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, mPosPacket);
PlayGamesPlatform.Instance.RealTime.SendMessage(true, participantId, mPosPacket);
PlayGamesPlatform.Instance.RealTime.SendMessage(false, participantId, mPosPacket);
}

public void SendMyMessage(string message)
{

byte[] mPosPacket = new byte[2];
mPosPacket[0] = (byte)'I';
mPosPacket[1] = (byte)'D';
PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, mPosPacket);

//byte[] myMessage = System.Text.Encoding.Unicode.GetBytes(message);
byte[] myMessage = System.Text.ASCIIEncoding.Default.GetBytes(message);
byte[] myMessage2 = Encoding.ASCII.GetBytes(message);

//bool reliable = false;
string participantId = playerOther.ParticipantId;
//messagesRecievedText.text = "SendMessage  to ID: " + playerOther.ParticipantId + "  <>  " + Time.deltaTime;
PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, myMessage);
PlayGamesPlatform.Instance.RealTime.SendMessage(true, participantId, myMessage);
PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, myMessage);
PlayGamesPlatform.Instance.RealTime.SendMessage(false, participantId, myMessage);

PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, myMessage2);
PlayGamesPlatform.Instance.RealTime.SendMessage(true, participantId, myMessage2);
PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, myMessage2);
PlayGamesPlatform.Instance.RealTime.SendMessage(false, participantId, myMessage2);

}

public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
{
if (data[0] == (byte)'I')
{
    messagesRecievedText.text = "FUCK YES BITCH!!!";
}
messagesRecievedText.text = "FUCK YES BITCH 2!!!";


string thisMessage = System.Text.Encoding.Default.GetString(data);
//string[] raw = thisMessage.Split(new string[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
messagesRecievedText.text += "recieved " + thisMessage  + " From: " + senderId;

string thisMessage2 = Encoding.ASCII.GetString(data);
messagesRecievedText.text += "recieved " + thisMessage2 + " From: " + senderId;

string thisMessage3 = Encoding.ASCII.GetString(data);
messagesRecievedText.text += "recieved " + thisMessage2 + " From: " + senderId;

}

private void Update()
{
//messagesRecievedText.text = "No messages Recieved :( " + Time.timeSinceLevelLoad;
//SendMyMessage("banana!!!!!!!");

debugText.text = "Currently connected Players:";
foreach (Participant p in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
{
    debugText.text += System.Environment.NewLine + p.DisplayName + " <> " + p.ParticipantId;
}
debugText.text += System.Environment.NewLine + "Countdown: " + countdownTimeMe;
debugText.text += System.Environment.NewLine + "CurrentDurationMe: " + currentDurationMe;
debugText.text += System.Environment.NewLine + "MultiplayerMode: " + multiplayerMode;
debugText.text += System.Environment.NewLine;
debugText.text += System.Environment.NewLine + "gameStarted: " + gameStarted.ToString();
debugText.text += System.Environment.NewLine + "GameOver: " + gameOver.ToString();
debugText.text += System.Environment.NewLine + "I Win: " + iWin.ToString();

debugText.text += System.Environment.NewLine + "Social ID Self: " + PlayGamesPlatform.Instance.localUser.userName + "  <>  " + PlayGamesPlatform.Instance.localUser.id;
debugText.text += System.Environment.NewLine + "playerMe: " + playerMe.DisplayName;
debugText.text += System.Environment.NewLine + "playerOther: " + playerOther.DisplayName;

debugText.text += System.Environment.NewLine;
debugText.text += System.Environment.NewLine + "Timesincestart: " + Time.timeSinceLevelLoad;

if (gameOver)
{
    if (iWin)
    {
        IWinPanel.SetActive(true);
        ILostPanel.SetActive(false);
    }
    else
    {
        IWinPanel.SetActive(false);
        ILostPanel.SetActive(true);
    }
}
else
{
    CheckOtherPlayerLeft();

    player1Taps.text = playerMe.DisplayName + ": " + tapCountMe;
    player2Taps.text = playerOther.DisplayName + ": " + tapCountOther;

    if (!gameStarted)
    {
        if (countdownTimeMe > 0.0f)
        {
            countdownTimeMe -= Time.deltaTime;
            countdownPanel.GetComponentInChildren<Text>().text = ((int)countdownTimeMe + 1).ToString();
        }
        else if (countdownTimeMe <= 0.0f && countdownTimeOther <= 0.0f)
        {
            gameStarted = true;
            if (countdownPanel.activeSelf)
            {
                countdownPanel.SetActive(false);
            }
        }
    }
    else
    {
        //Standard Mode
        if (multiplayerMode == 0 || multiplayerMode == 1 || multiplayerMode == 2)
        {
            currentDurationMe -= Time.deltaTime;

            timeSlider.GetComponent<Slider>().value = currentDurationMe / startDuration;

            timeSlider.GetComponentInChildren<Text>().text =
                (int)(timeSlider.GetComponent<Slider>().value * startDuration) + "s";

            checkWinLossStandard();
        }
    }
}
}

void CheckOtherPlayerLeft()
{
if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count == 1)
{
    gameOver = true;
    iWin = true;
}
}

public void IncrementMyTapCount()
{
tapCountMe++;
//TODO: Send info to other player;

SendMyMessage("tapCount:" + tapCountMe);
}

void checkWinLossStandard()
{    
if (currentDurationMe < 0.0f && currentDurationOther < 0.0f){

    gameOver = true;

    //  I Win
    if (tapCountMe > tapCountOther)
    {
        iWin = true;
    }
    else //  I Lose
    {
        iWin = false;
    }
}
}

public void LeaveGame()
{
PlayGamesPlatform.Instance.RealTime.LeaveRoom();
SceneManager.LoadScene(0);
//if (playerMe.IsConnectedToRoom)
//{
//    PlayGamesPlatform.Instance.RealTime.LeaveRoom();
//}
}


public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
{
    if (data[0] == (byte)'I')
    {
        debugText.text = "Recieved " + data[1].ToString() + " from: " + senderId;
        debugText.text += System.Environment.NewLine + "Reliable: " + isReliable;
        foreach (Participant p in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
        {
            debugText.text += System.Environment.NewLine + p.DisplayName + " <> " + p.ParticipantId;
        }
    }
}

public void LeaveGame()
{
    PlayGamesPlatform.Instance.RealTime.LeaveRoom();
    SceneManager.LoadScene(0);
}

public void SendShit()
{
    if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count == 2)
    {
        debugText.text = "Send Shit  " + Time.timeSinceLevelLoad + " <> " +  PlayGamesPlatform.Instance.RealTime.GetSelf();
        string participantId = "";

        foreach (Participant p in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
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
        //PlayGamesPlatform.Instance.RealTime.SendMessage(true, participantId, mPosPacket);
        PlayGamesPlatform.Instance.RealTime.SendMessage(false, participantId, mPosPacket);
    }
}

public void OnRoomSetupProgress(float percent)
{
    throw new NotImplementedException();
}

public void OnRoomConnected(bool success)
{
    throw new NotImplementedException();
}

public void OnLeftRoom()
{

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
*/
