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

    public GameObject raceSliderMe;
    public GameObject raceSliderOther;

    public float currentDurationMe;
    public float currentDurationOther;
    public int tapCountMe;
    public int tapCountOther;

    public Participant playerMe;
    public Participant playerOther;

    float startDuration;

    public bool gameOver;
    public bool iWin;
    public bool iDraw;

    public GameObject IWinPanel;
    public GameObject ILostPanel;
    public GameObject DrawPanel;

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
        iDraw = false;

        IWinPanel.SetActive(false);
        ILostPanel.SetActive(false);
        DrawPanel.SetActive(false);
        timeSlider.GetComponent<Slider>().value = 1.0f;
        //messagesRecievedText.text = "No messages Received :(";
    }

    void checkWinLoss()
    {
        gameOver = true;

        if (tapCountMe > tapCountOther)  // I Win
        {
            iWin = true;
        }
        else if (tapCountMe == tapCountOther)  // I Draw
        {
            iDraw = true;
        }
        else  // I Lost
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

    void SetStandardMode()
    {
        gameModeText.gameObject.SetActive(true);

        playerMeTaps.gameObject.SetActive(true);
        playerOtherTaps.gameObject.SetActive(true);
        playerMeTaps.text = playerMe.DisplayName + ": " + tapCountMe;
        playerOtherTaps.text = playerOther.DisplayName + ": " + tapCountOther;

        timeSlider.SetActive(true);
        raceSliderMe.SetActive(false);
        raceSliderOther.SetActive(false);
        timeSlider.GetComponentInChildren<Text>().text = startDuration + "s";
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
                        SetStandardMode();
                        break;
                    case 1: //Standard Mode 30s
                        startDuration = 30.0f;
                        currentDurationMe = 30.0f;
                        gameModeText.text = "Game Mode: Standard 30";
                        SetStandardMode();
                        break;
                    case 2: //Standard Mode 60s
                        startDuration = 60.0f;
                        currentDurationMe = 60.0f;
                        gameModeText.text = "Game Mode: Standard 60";
                        SetStandardMode();
                        break;
                    case 3:  //Race Mode
                        timeSlider.SetActive(false);
                        gameModeText.gameObject.SetActive(false);

                        playerMeTaps.gameObject.SetActive(false);
                        playerOtherTaps.gameObject.SetActive(false);

                        raceSliderMe.GetComponentInChildren<Text>().text = playerMe.DisplayName + ": " + tapCountMe;
                        raceSliderOther.GetComponentInChildren<Text>().text = playerOther.DisplayName + ": " + tapCountOther;

                        raceSliderMe.SetActive(true);
                        raceSliderMe.GetComponent<Slider>().value = 0.0f;
                        raceSliderOther.SetActive(true);
                        raceSliderOther.GetComponent<Slider>().value = 0.0f;
                        //gameModeText.text = "Game Mode: Race";
                        break;
                    case 4:  //Balance Mode
                        timeSlider.SetActive(false);
                        gameModeText.gameObject.SetActive(false);

                        playerMeTaps.gameObject.SetActive(false);
                        playerOtherTaps.gameObject.SetActive(false);

                        raceSliderMe.SetActive(false);
                        raceSliderOther.SetActive(false);
                        //gameModeText.text = "Game Mode: Balance";
                        break;
                }
            }

            if (gameOver)
            {
                if (iDraw)
                {
                    IWinPanel.SetActive(false);
                    ILostPanel.SetActive(false);
                    DrawPanel.SetActive(true);
                }
                else if (iWin)
                {
                    IWinPanel.SetActive(true);
                    ILostPanel.SetActive(false);
                    DrawPanel.SetActive(false);
                }
                else
                {
                    IWinPanel.SetActive(false);
                    ILostPanel.SetActive(true);
                    DrawPanel.SetActive(false);
                }
            }
            else
            {
                CheckOtherPlayerLeft();

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
                        playerMeTaps.text = playerMe.DisplayName + ": " + tapCountMe;
                        playerOtherTaps.text = playerOther.DisplayName + ": " + tapCountOther;

                        if (currentDurationMe < 0.0f)
                        {
                            gpsm.SendMyMessage("CD:" + currentDurationMe.ToString(), true);
                            if (currentDurationOther < 0.0f)
                            {
                                checkWinLoss();
                            }
                        }
                        else
                        {
                            currentDurationMe -= Time.deltaTime;

                            timeSlider.GetComponent<Slider>().value = currentDurationMe / startDuration;

                            timeSlider.GetComponentInChildren<Text>().text =
                                (int)(timeSlider.GetComponent<Slider>().value * startDuration) + "s";
                        }
                    }
                    // Race Mode
                    else if (multiplayerMode == 3)
                    {
                        raceSliderMe.GetComponent<Slider>().value = tapCountMe / 200.0f;
                        raceSliderOther.GetComponent<Slider>().value = tapCountOther / 200.0f;

                        raceSliderMe.GetComponentInChildren<Text>().text = playerMe.DisplayName + ": " + tapCountMe;
                        raceSliderOther.GetComponentInChildren<Text>().text = playerOther.DisplayName + ": " + tapCountOther;

                        if (tapCountMe  == 200 ||  tapCountOther == 200)
                        {
                            checkWinLoss();
                        }
                    }
                }
            }
        }
    }
}