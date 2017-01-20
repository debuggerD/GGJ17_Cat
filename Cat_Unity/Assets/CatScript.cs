using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatScript : MonoBehaviour {

    Rigidbody rigidbody;
    CharacterController controller;
	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    Vector3 lastValidPosition;
    void Update() {
        if (Input.GetKey("w"))
        {
            controller.Move(new Vector3(1, 0, 0) * Time.deltaTime);
        }
        if (Input.GetKey("a"))
        {
            controller.Move(new Vector3(0, 0, 1) * Time.deltaTime);

        }
        if (Input.GetKey("s"))
        {
            controller.Move(new Vector3(-1, 0, 0) * Time.deltaTime);
        }
        if (Input.GetKey("d"))
        {
            controller.Move(new Vector3(0, 0, -1) * Time.deltaTime);
        }
    }
    void FixedUpdate()
    {

    }


    void OnCollisionEnter(Collision collision)
    {
        print("collision!!");
        if (collision.gameObject.tag == "Wall")
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

}
