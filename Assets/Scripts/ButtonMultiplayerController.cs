using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMultiplayerController : MonoBehaviour
{
    public GameObject myButton;
    public GameObject soundClick;
    MultiplayerController mc;
    MenuManager mm;

    void Start()
    {
        mc = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MultiplayerController>();
        mm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MenuManager>();
    }

    void Update()
    {
        if (!mc.gameOver && mc.gameStarted && mc.currentDurationMe > 0.0f && mc.countdownTimeMe <= 0.0f &&
            PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count == 2 && !mm.isSingleplayer)
        {
            if (Input.touchCount > 0 && Application.isEditor)
            {
                if (Input.touches[0].phase.Equals(TouchPhase.Began))
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                    RaycastHit2D hit = Physics2D.Raycast(pos - Vector3.forward, new Vector3(0, 0, 1));
                    if (hit.collider != null && hit.collider.tag.Equals("Button") &&
                        hit.collider.gameObject.Equals(myButton))
                    {
                        //Increments tap count and send info to other player
                        mc.IncrementMyTapCount();

                        GameObject myClick = Instantiate(soundClick) as GameObject;
                        Destroy(myClick, 1.0f);

                        myButton.GetComponent<Animator>().SetTrigger("Tap");
                        myButton.GetComponent<Animator>().SetTrigger("TapReturn");
                    }
                }
            }
            else if (Input.touchCount == 0)
            {
                myButton.GetComponent<Animator>().SetTrigger("TapReturn");
            }

            //Only used in Unity Editor for texting purposes
            if (Input.GetMouseButtonDown(0))
            {
                //Increments tap count and send info to other player
                mc.IncrementMyTapCount();

                GameObject myClick = Instantiate(soundClick) as GameObject;
                Destroy(myClick, 1.0f);

                myButton.GetComponent<Animator>().SetTrigger("Tap");
                myButton.GetComponent<Animator>().SetTrigger("TapReturn");
            }
            else if (Input.GetMouseButtonUp(0))
            {
                myButton.GetComponent<Animator>().SetTrigger("TapReturn");
            }
        }
    }
}