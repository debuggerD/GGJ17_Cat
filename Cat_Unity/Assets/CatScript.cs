using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (Input.GetKey("w"))
        {
            transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("a"))
        {
            transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("d"))
        {
            transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime, Space.World);
        }
    }
    void FixedUpdate()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        print("trigger!!");
        if (col.gameObject.tag == "Wall")
        {
            Destroy(col.gameObject);
        }
    }

}
