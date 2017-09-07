using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {

    public int tapCount;
    public GameObject myButton;

    public TextMesh tapScoreText;
    int fingerId;
    bool fingerLocked;

    public Text myTotalTapText;

    GameManager gm;
    public GameObject soundClick;
    void Start() {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (PlayerPrefs.GetInt("unlimitedRound") == 0)
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

        fingerLocked = false;
    }

    void UpdateScore() {
        if (gm.isUnlimited == 0)
        {
            tapScoreText.text = "TAPS: " + tapCount;
            myTotalTapText.text = "";
        }
        else
        {
            gm.incrementTapCount();
            tapScoreText.text = "";
            myTotalTapText.text = "TOTAL TAPS: " + PlayerPrefs.GetInt("TapCount");
        }
    }

    void CheckBestScore()
    {
        switch (gm.roundDuration)
        {
            case 15:
                if (tapCount > PlayerPrefs.GetInt("Best15"))
                {
                    PlayerPrefs.SetInt("Best15", tapCount);
                    gm.BestScoreLimited.text = "Best Score: " + PlayerPrefs.GetInt("Best15").ToString();
                }
                break;
            case 30:
                if (tapCount > PlayerPrefs.GetInt("Best30"))
                {
                    PlayerPrefs.SetInt("Best30", tapCount);
                    gm.BestScoreLimited.text = "Best Score: " + PlayerPrefs.GetInt("Best30").ToString();
                }
                break;
            case 60:
                if (tapCount > PlayerPrefs.GetInt("Best60"))
                {
                    PlayerPrefs.SetInt("Best60", tapCount);
                    gm.BestScoreLimited.text = "Best Score: " + PlayerPrefs.GetInt("Best60").ToString();
                }
                break;
        }
    }

    void Update() {
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
                        fingerId = Input.touches[0].fingerId;
                        fingerLocked = true;

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
        }
    }
}
