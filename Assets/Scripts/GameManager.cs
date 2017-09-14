using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;

public class GameManager : MonoBehaviour {

    public int roundDuration;
    float startTime;
    public Text mySeconds;

    public Text BestScoreLimited;

    public bool startedRound;
    public GameObject startRoundButton;
    public Slider timeSlider;

    public int tapCountUnlimited;

    public ButtonController bc;

    public int isUnlimited;
    private bool fillingBack;

    void Start () {
        startedRound = false;
        roundDuration = PlayerPrefs.GetInt("roundDuration");
        mySeconds.text = roundDuration.ToString() + "s";
        isUnlimited = PlayerPrefs.GetInt("unlimitedRound");

        tapCountUnlimited = PlayerPrefs.GetInt("TapCount");

        switch (roundDuration)
        {
            case 15:
                BestScoreLimited.text = "Best Score: " + PlayerPrefs.GetInt("Best15").ToString();
                break;
            case 30:
                BestScoreLimited.text = "Best Score: " + PlayerPrefs.GetInt("Best30").ToString();
                break;
            case 60:
                BestScoreLimited.text = "Best Score: " + PlayerPrefs.GetInt("Best60").ToString();
                break;
        }

        if (isUnlimited == 1)
        {
            timeSlider.gameObject.SetActive(false);
            mySeconds.gameObject.SetActive(false);
            startRoundButton.SetActive(false);
            BestScoreLimited.gameObject.SetActive(true);
            startedRound = true;
        }
    }

    public void incrementTapCount()
    {   
        tapCountUnlimited++;
        PlayerPrefs.SetInt("TapCount", tapCountUnlimited);
    }

    public void StartRound()
    {
        startedRound = true;
        startTime = Time.time;
        startRoundButton.SetActive(false);
    }


    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
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
        else if (startedRound && isUnlimited == 0) 
        {
            if (Time.time - startTime < roundDuration)
            {
                mySeconds.text = (roundDuration - (int)(Time.time - startTime)).ToString();
                timeSlider.value = 1 - (Time.time - startTime) / roundDuration;
            }
            else //Round Ended
            {
                UpdateBestScores();

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
                }
                break;
            case 30:
                if (PlayerPrefs.GetInt("Best30") <= bc.tapCount)
                {
                    PlayerPrefs.SetInt("Best30", bc.tapCount);
                }
                break;
            case 60:
                if (PlayerPrefs.GetInt("Best60") <= bc.tapCount)
                {
                    PlayerPrefs.SetInt("Best60", bc.tapCount);
                }
                break;
        }
    }
}