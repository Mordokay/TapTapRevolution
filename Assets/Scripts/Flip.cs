using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flip : MonoBehaviour {

    void Update () {
        if (Input.touchCount == 1)
        {
            this.transform.Rotate(0f, 0f, 2f);
        }
        else if(Input.touchCount == 2)
        {
            this.transform.Rotate(0f, 0f, -2f);
        }
	}
}
