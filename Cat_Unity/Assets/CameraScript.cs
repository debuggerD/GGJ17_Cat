using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    GameObject cat;
	// Use this for initialization
	void Start () {
        cat = GameObject.Find("Cat");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = cat.transform.position + new Vector3(-5f, 4.5f, -5f);
	}
}
