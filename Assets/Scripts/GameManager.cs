using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;

public class GameManager : MonoBehaviour {

    public int roundDuration;
    public float startTime;
    public Text mySeconds;

    public Text BestScoreLimited;

    public bool startedRound;
    public GameObject startRoundButton;
    public Slider timeSlider;

    public int tapCountUnlimited;

    public ButtonController bc;
    public GameObject button;
    public ClickRecorder cr;

    public bool isUnlimited;
    private bool fillingBack;
    MenuManager mm;

    public GameObject MenuCanvas;
    public GameObject mainMenu;
    public GameObject singlePlayerGameCanvas;
    public GameObject GameComponents;

    void Start () {
        mm = this.GetComponent<MenuManager>(); 
    }

    public void StartSingleRound()
    {
        fillingBack = false;
        isUnlimited = mm.isUnlimited;
        startRoundButton.SetActive(true);

        if (isUnlimited)
        {
            tapCountUnlimited = PlayerPrefs.GetInt("TapCount");

            bc.myTotalTapText.text = "TOTAL TAPS: " + PlayerPrefs.GetInt("TapCount");

            timeSlider.gameObject.SetActive(false);
            mySeconds.gameObject.SetActive(false);
            startRoundButton.SetActive(false);
            BestScoreLimited.gameObject.SetActive(true);
            startedRound = true;
        }
        else
        {
            startedRound = false;
            roundDuration = mm.roundDuration;
            mySeconds.text = roundDuration.ToString() + "s";

            timeSlider.gameObject.SetActive(true);
            timeSlider.value = 1.0f;

            switch (roundDuration)
            {
                case 15:
                    BestScoreLimited.text = "Best Score: " + PlayerPrefs.GetInt("Best15").ToString();
                    Debug.Log("Best Score: " + PlayerPrefs.GetInt("Best15").ToString());
                    break;
                case 30:
                    BestScoreLimited.text = "Best Score: " + PlayerPrefs.GetInt("Best30").ToString();
                    Debug.Log("Best Score: " + PlayerPrefs.GetInt("Best30").ToString());
                    break;
                case 60:
                    BestScoreLimited.text = "Best Score: " + PlayerPrefs.GetInt("Best60").ToString();
                    Debug.Log("Best Score: " + PlayerPrefs.GetInt("Best60").ToString());
                    break;
            }
        }
    }

    public void incrementTapCount()
    {   
        tapCountUnlimited++;
        PlayerPrefs.SetInt("TapCount", tapCountUnlimited);
        this.GetComponent<GooglePlayServicesManager>().UpdateLeaderboards();
    }

    public void StartRound()
    {
        startedRound = true;
        startTime = Time.time;
        startRoundButton.SetActive(false);
    }

    public void BackToMenu()
    {
        button.transform.localPosition = new Vector3(0.0f, 3.28f, 0.0f);
        MenuCanvas.SetActive(true);
        mainMenu.SetActive(true);
        singlePlayerGameCanvas.SetActive(false);
        GameComponents.SetActive(false);
        mm.isSingleplayer = false;
        mm.isUnlimited = false;
        UpdateBestScores();

    }

	void Update () {
        if (fillingBack)
        {
            timeSlider.value += (Time.deltaTime / 2.0f);

            if (timeSlider.value >= 1.0f)
            {
                timeSlider.value = 1.0f;
                fillingBack = false;
                startRoundButton.SetActive(true);
            }
        }
        else if (startedRound && !isUnlimited) 
        {
            if (Time.time - startTime < roundDuration)
            {
                mySeconds.text = (roundDuration - (int)(Time.time - startTime)).ToString();
                timeSlider.value = 1 - (Time.time - startTime) / roundDuration;
            }
            else //Round Ended
            {
                UpdateBestScores();
                this.GetComponent<GooglePlayServicesManager>().UpdateLeaderboards();

                bc.tapCount = 0;
                startedRound = false;

                mySeconds.text = roundDuration.ToString() + "s";
                fillingBack = true;
            }
        }
	}

    private void UpdateBestScores()
    {
        switch (roundDuration)
        {
            case 15:
                if (PlayerPrefs.GetInt("Best15") <= bc.tapCount)
                {
                    PlayerPrefs.SetInt("Best15", bc.tapCount);
                    this.GetComponent<GooglePlayServicesManager>().UpdateLeaderboards();
                }
                break;
            case 30:
                if (PlayerPrefs.GetInt("Best30") <= bc.tapCount)
                {
                    PlayerPrefs.SetInt("Best30", bc.tapCount);
                    this.GetComponent<GooglePlayServicesManager>().UpdateLeaderboards();
                }
                break;
            case 60:
                if (PlayerPrefs.GetInt("Best60") <= bc.tapCount)
                {
                    PlayerPrefs.SetInt("Best60", bc.tapCount);
                    this.GetComponent<GooglePlayServicesManager>().UpdateLeaderboards();
                }
                break;
        }
    }
}