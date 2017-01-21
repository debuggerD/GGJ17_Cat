using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScientistScript : MonoBehaviour {

    float speed = 1.0f;
    GameObject Cat;
    CharacterController controller;
	// Use this for initialization
	void Start () {
        Cat = GameObject.FindWithTag("Player");
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        Chase();
		
	}

    void Chase()
    {
        controller.Move(new Vector3(Cat.transform.position.x - transform.position.x, 0, Cat.transform.position.z - transform.position.z).normalized * Time.deltaTime * speed);
    }

    void Roam()
    {
        controller.Move(new Vector3(1, 0, -1) * Time.deltaTime * speed);

    }
}
