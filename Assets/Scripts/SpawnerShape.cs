﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerShape : MonoBehaviour {

    public float speed;

	void Start () {
        GameObject myShape = Instantiate(Resources.Load("Shapes")) as GameObject;
        myShape.transform.position = Vector3.zero;
        myShape.GetComponent<Rigidbody2D>().AddForce(Vector2.right * speed);
        Destroy(myShape, 22.0f);

        InvokeRepeating("SpawnShape", 0.0f, 10.0f);
	}
	
    void SpawnShape()
    {
        GameObject myShape = Instantiate(Resources.Load("Shapes")) as GameObject;
        myShape.transform.position = new Vector3(-27.0f, 0.0f, 0.0f);
        myShape.GetComponent<Rigidbody2D>().AddForce(Vector2.right * speed);
        Destroy(myShape, 22.0f);
    }
}
