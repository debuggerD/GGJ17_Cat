using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatScript : MonoBehaviour {

    Rigidbody rigid;
    CharacterController controller;
    float speed = 2.0f;
	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    Vector3 lastValidPosition;
    void Update() {
        if (Input.GetKey("w"))
        {
            controller.Move(new Vector3(1, 0, 1) * Time.deltaTime * speed);
        }
        if (Input.GetKey("a"))
        {
            controller.Move(new Vector3(-1, 0, 1) * Time.deltaTime * speed);
        }
        if (Input.GetKey("s"))
        {
            controller.Move(new Vector3(-1, 0, -1) * Time.deltaTime * speed);
        }
        if (Input.GetKey("d"))
        {
            controller.Move(new Vector3(1, 0, -1) * Time.deltaTime * speed);
        }
        if (Input.GetKeyDown("space"))
        {
            controller.Move(new Vector3(Random.Range(-2.0f, 2.0f), 0, Random.Range(-2.0f, 2.0f)));
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
            rigid.velocity = Vector3.zero;
        }
    }

}
