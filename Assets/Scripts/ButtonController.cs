using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {

    public int tapCount;
    public GameObject myButton;

    public TextMesh tapScoreText;

    public Text myTotalTapText;

    GameManager gm;
    MenuManager mm;

    public GameObject soundClick;

    void Start() {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        mm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MenuManager>();
    }

    public void StartButtonController()
    {
       // myButton.GetComponent<Animator>().SetTrigger("Tap");

        if (gm == null || mm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            mm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MenuManager>();
        }
        if (mm.isSingleplayer)
        {
            if (!mm.isUnlimited)
            {
                tapCount = 0;
                tapScoreText.text = "TAPS: " + tapCount;
                myTotalTapText.text = "";
            }
            else
            {
                tapScoreText.text = "";
                myTotalTapText.text = "TOTAL TAPS: " + PlayerPrefs.GetInt("TapCount");
            }
        }
    }

    void UpdateScore() {
        if (!gm.isUnlimited )
        {
            //Debug.Log("tapCount: " + tapCount);
            tapScoreText.text = "TAPS: " + tapCount;
            myTotalTapText.text = "";

            if (PlayGamesPlatform.Instance.localUser.authenticated)
            {
                this.GetComponent<ClickRecorder>().AddLine(Time.time - gm.startTime);
            }

#if UNITY_EDITOR
            this.GetComponent<ClickRecorder>().AddLine(Time.time - gm.startTime);
#endif

        }
        else
        {
            gm.incrementTapCount();
            tapScoreText.text = "";
            myTotalTapText.text = "TOTAL TAPS: " + PlayerPrefs.GetInt("TapCount");
        }
    }

    public void UnlockAchievement(string id)
    {
        PlayGamesPlatform.Instance.ReportProgress(id, 100, success => { });
    }

    void CheckBestScore()
    {
        switch (gm.roundDuration)
        {
            case 15:
                if (tapCount > PlayerPrefs.GetInt("Best15"))
                {
                    if(tapCount == 50)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_the_50_in_15);
                    }
                    else if(tapCount == 100)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_the_100_in_15);
                    }
                    else if (tapCount == 150)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_the_150_in_15);
                    }
                    else if (tapCount == 200)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_the_200_in_15);
                    }
                    else if (tapCount > 250)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_you_are_15__awesome);
                    }
                    PlayerPrefs.SetInt("Best15", tapCount);
                    gm.BestScoreLimited.text = "Best Score: " + PlayerPrefs.GetInt("Best15").ToString();
                }
                break;
            case 30:
                if (tapCount > PlayerPrefs.GetInt("Best30"))
                {
                    if (tapCount == 100)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_the_100_in_30);
                    }
                    else if (tapCount == 200)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_the_200_in_30);
                    }
                    else if (tapCount == 300)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_the_300_in_30);
                    }
                    else if (tapCount == 400)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_the_400_in_30);
                    }
                    else if (tapCount > 500)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_you_are_30_awesome);
                    }
                    PlayerPrefs.SetInt("Best30", tapCount);
                    gm.BestScoreLimited.text = "Best Score: " + PlayerPrefs.GetInt("Best30").ToString();
                }
                break;
            case 60:
                if (tapCount > PlayerPrefs.GetInt("Best60"))
                {
                    if (tapCount == 200)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_the_200_in_60);
                    }
                    else if (tapCount == 400)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_the_400_in_60);
                    }
                    else if (tapCount == 600)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_the_600_in_60);
                    }
                    else if (tapCount == 800)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_the_800_in_60);
                    }
                    else if (tapCount > 1000)
                    {
                        UnlockAchievement(TapTapRevolutionResources.achievement_you_are_60_awesome);
                    }

                    PlayerPrefs.SetInt("Best60", tapCount);
                    gm.BestScoreLimited.text = "Best Score: " + PlayerPrefs.GetInt("Best60").ToString();
                }
                break;
        }
    }

    void Update() {
        if (mm.isSingleplayer)
        {
            if (gm.startedRound)
            {
                if (Input.touchCount > 0)
                {
                    if (Input.touches[0].phase.Equals(TouchPhase.Began))
                    {
                        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                        RaycastHit2D hit = Physics2D.Raycast(pos - Vector3.forward, new Vector3(0, 0, 1));
                        if (hit.collider != null && hit.collider.tag.Equals("Button") &&
                            hit.collider.gameObject.Equals(myButton))
                        {
                            tapCount++;

                            GameObject myClick = Instantiate(soundClick) as GameObject;
                            Destroy(myClick, 1.0f);

                            CheckBestScore();
                            UpdateScore();

                            myButton.GetComponent<Animator>().SetTrigger("Tap");
                            myButton.GetComponent<Animator>().SetTrigger("TapReturn");
                        }
                    }
                }
                else if (Input.touchCount == 0)
                {
                    myButton.GetComponent<Animator>().SetTrigger("TapReturn");
                }
#if UNITY_EDITOR
                //Only used in Unity Editor for testing purposes
                if (Input.GetMouseButtonDown(0))
                {
                    tapCount++;

                    GameObject myClick = Instantiate(soundClick) as GameObject;
                    Destroy(myClick, 1.0f);

                    CheckBestScore();
                    UpdateScore();

                    myButton.GetComponent<Animator>().SetTrigger("Tap");
                    myButton.GetComponent<Animator>().SetTrigger("TapReturn");
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    myButton.GetComponent<Animator>().SetTrigger("TapReturn");
                }
#endif
            }
        }
    }
}
