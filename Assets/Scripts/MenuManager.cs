﻿using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public GameObject MenuCanvas;
    public GameObject SinglePlayerGameCanvas;
    public GameObject GameComponents;

    public GameObject mainMenu;
    public GameObject singleplayerMenu;
    public GameObject multiplayerMenu;
    public GameObject options;

    public GameObject MutedButton;
    public GameObject UnmutedButton;

    public Text multiplayerFeedbackText;

    public Slider soundSlider;
    public float lastVolume;
    public bool wasMutedBefore;

    public GameObject multiplayerMainPanel;
    public GameObject multiplayerOptions;
    public GameObject standardPanel;

    public bool isSingleplayer;
    public bool isUnlimited;
    public int singleLimitedType;
    public int roundDuration;

    public ClickRecorder cr;
    public ButtonController bc;

    private void Start()
    {
        isSingleplayer = false;
        isUnlimited = false;

        GameObject[] mySoundtracks = GameObject.FindGameObjectsWithTag("Soundtrack");
        for(int i = 1; mySoundtracks.Length > 1 && i < mySoundtracks.Length; i++)
        {
            Destroy(mySoundtracks[i]);
        }
        if(mySoundtracks.Length == 1)
        {
            DontDestroyOnLoad(mySoundtracks[0]);
        }

        lastVolume = PlayerPrefs.GetFloat("volume");
        AudioListener.volume = PlayerPrefs.GetFloat("volume");
        soundSlider.value = PlayerPrefs.GetFloat("volume");
        if (PlayerPrefs.GetInt("sound") == 0)
        {
            MuteSound();
            wasMutedBefore = true;
        }
        else
        {
            wasMutedBefore = false;
            AudioListener.pause = false;
        }
    }

    public void ShowSinglePlayer()
    {
        mainMenu.SetActive(false);
        singleplayerMenu.SetActive(true);
    }
    public void ShowMultiplayer()
    {
        mainMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
        multiplayerFeedbackText.text = "";
    }
    public void ShowOptions()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        singleplayerMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        options.SetActive(false);
    }

    public void MuteSound()
    {
        if (lastVolume == -99)
        {
            lastVolume = soundSlider.value;
        }
        PlayerPrefs.SetInt("sound", 0);
        AudioListener.pause = true;
        soundSlider.value = 0;
        MutedButton.SetActive(false);
        UnmutedButton.SetActive(true);
    }

    //sound returns at where it wasbefore he was muted
    public void UnmuteSound()
    {
        PlayerPrefs.SetInt("sound", 1);
        AudioListener.pause = false;
        soundSlider.value = lastVolume;
        MutedButton.SetActive(true);
        UnmutedButton.SetActive(false);
        lastVolume = -99;
    }

    public void ChangeVolume()
    {
        AudioListener.volume = soundSlider.value;
        PlayerPrefs.SetFloat("volume", soundSlider.value);
        if(soundSlider.value == 0)
        {
            PlayerPrefs.SetInt("sound", 0);
            wasMutedBefore = true;
            MuteSound();
        }
        else if(wasMutedBefore)
        {
            AudioListener.pause = false;
            MutedButton.SetActive(true);
            UnmutedButton.SetActive(false);
            wasMutedBefore = false;
            PlayerPrefs.SetInt("sound", 1);
        }
    }

    public void ResetStats()
    {
        PlayerPrefs.SetInt("BestScore", 0);
        PlayerPrefs.SetInt("Best15", 0);
        PlayerPrefs.SetInt("Best30", 0);
        PlayerPrefs.SetInt("Best60", 0);
        PlayerPrefs.SetInt("TapCount", 0);
    }

    public void LoadUnlimtedRound()
    {
        isSingleplayer = true;
        isUnlimited = true;
        singleplayerMenu.SetActive(false);
        SinglePlayerGameCanvas.SetActive(true);
        GameComponents.SetActive(true);
        this.GetComponent<GameManager>().StartSingleRound();
        bc.StartButtonController();
    }

    public void LoadTimeRound(int time)
    {
        isSingleplayer = true;
        isUnlimited = false;

        switch (time)
        {
            case 15:
                singleLimitedType = 0;
                break;
            case 30:
                singleLimitedType = 1;
                break;
            case 60:
                singleLimitedType = 2;
                break;
        }
        roundDuration = time;

        isSingleplayer = true;
        isUnlimited = false;
        singleplayerMenu.SetActive(false);
        SinglePlayerGameCanvas.SetActive(true);
        GameComponents.SetActive(true);

        cr.AddNewTable();
        this.GetComponent<GameManager>().StartSingleRound();
        bc.StartButtonController();
    }

    public void LeaveMultiplayerOptions()
    {
        multiplayerMainPanel.SetActive(true);
        multiplayerOptions.SetActive(false);
    }

    public void LeaveMultiplayerStandard()
    {
        standardPanel.SetActive(false);
        multiplayerOptions.SetActive(true);

        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
    }

    public void ShowMultiplayerOptions()
    {
        multiplayerMainPanel.SetActive(false);
        multiplayerOptions.SetActive(true);

        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
    }

    public void ShowMultiplayerStandard()
    {
        standardPanel.SetActive(true);
        multiplayerOptions.SetActive(false);
    }
}
