using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScientistScript : MonoBehaviour {

    public Sprite front;
    public Sprite back;
    float speed = 1.0f;
    float angularSpeed = 1.0f;
    GameObject Cat;
    CharacterController controller;
    SpriteRenderer renderer;
    GameObject cam;
    class patrolBehavior
    {
        public Vector2 destination;
        public float stopDuration;
        public patrolBehavior(Vector2 dest, float stopDuration)
        {
            this.destination = dest;
            this.stopDuration = stopDuration;
        }
    }
    List<patrolBehavior> patrolPoints;

    bool chasing = false;
    float remainTime = 0;
    int patrolIndex = 0;

	// Use this for initialization
	void Start () {
        Cat = GameObject.FindWithTag("Player");
        cam = GameObject.FindWithTag("MainCamera");
        controller = GetComponent<CharacterController>();
        renderer = GetComponent<SpriteRenderer>();
        patrolPoints = new List<patrolBehavior>();
        patrolPoints.Add(new patrolBehavior(new Vector2(1.5f, 1.5f), 1.0f));
        patrolPoints.Add(new patrolBehavior(new Vector2(1.5f, 3.5f), 1.0f));
        patrolPoints.Add(new patrolBehavior(new Vector2(2.5f, 3.5f), 1.0f));
        patrolPoints.Add(new patrolBehavior(new Vector2(2.5f, 1.5f), 1.0f));

    }

    // Update is called once per frame
    void Update () {
        transform.rotation = cam.transform.rotation;
        if (chasing)
        {
            Chase();
        }
        else
        {
            Patrol();
        }
        if (Mathf.Abs(controller.velocity.x) < Mathf.Abs(controller.velocity.z) && controller.velocity.z > 0)
        {
            renderer.sprite = back;
            renderer.flipX = true;
        }
        else if (Mathf.Abs(controller.velocity.x) < Mathf.Abs(controller.velocity.z) && controller.velocity.z < 0)
        {
            renderer.sprite = front;
            renderer.flipX = false;
        }
        else if (Mathf.Abs(controller.velocity.x) > Mathf.Abs(controller.velocity.z) && controller.velocity.x > 0)
        {
            renderer.sprite = front;
            renderer.flipX = true;
        }
        else if (Mathf.Abs(controller.velocity.x) > Mathf.Abs(controller.velocity.z) && controller.velocity.x < 0)
        {
            renderer.sprite = back;
            renderer.flipX = false;
        }
    }

    void Chase()
    {
        controller.Move(new Vector3(Cat.transform.position.x - transform.position.x, 0, Cat.transform.position.z - transform.position.z).normalized * Time.deltaTime * speed);
    }

    void Patrol()
    {
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), patrolPoints[patrolIndex].destination) <= 0.2f)
        {
            if (remainTime > 0)
            {
                remainTime -= Time.deltaTime;
            }
            else
            {
                patrolIndex += 1;
                if (patrolIndex >= patrolPoints.Count)
                {
                    patrolIndex = 0;
                }
                remainTime = patrolPoints[patrolIndex].stopDuration;

            }
        }
        else
        {
            controller.Move((new Vector3(patrolPoints[patrolIndex].destination.x - transform.position.x, 0, patrolPoints[patrolIndex].destination.y - transform.position.z)).normalized * Time.deltaTime * speed);
        }
    }
}
