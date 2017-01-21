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

        // Initialize Logical Position
        transform.position = new Vector3(GameManagerScript.GetXPos(m_kCatLogicalStartI, m_kCatLogicalStartJ), 0.5f, GameManagerScript.GetZPos(m_kCatLogicalStartI, m_kCatLogicalStartJ));
    }

    // Update is called once per frame
    Vector3 lastValidPosition;
    void Update() {

        // Move by WASD
        Vector3 moveDir = new Vector3(0.0f, 0.0f, 0.0f);
        if (Input.GetKey("w"))
        {
            moveDir.x -= 1.0f;
            moveDir.z += 1.0f;
        }
        if (Input.GetKey("a"))
        {
            moveDir.x -= 1.0f;
            moveDir.z -= 1.0f;
        }
        if (Input.GetKey("s"))
        {
            moveDir.x += 1.0f;
            moveDir.z -= 1.0f;
        }
        if (Input.GetKey("d"))
        {
            moveDir.x += 1.0f;
            moveDir.z += 1.0f;
        }
        if (moveDir.sqrMagnitude > float.Epsilon)
        {
            moveDir.Normalize();
            controller.Move(moveDir * Time.deltaTime * speed);
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


    //////////////////////////////////////////////////////////////////////////////// 임시 코드데이터
    protected const int m_kCatLogicalStartI = 2;
    protected const int m_kCatLogicalStartJ = 19;
}
