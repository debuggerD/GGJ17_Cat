using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScientistScript : MonoBehaviour {

    public Sprite front;
    public Sprite back;
    public GameObject sightPrefab;
    float speed = 1.0f;
    float angularSpeed = 1.0f;
    GameObject Cat;
    CharacterController controller;
    SpriteRenderer spriteRenderer;
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
    float remainTime = 1;
    int patrolIndex = 0;
    public Quaternion facingQuat = Quaternion.Euler(Vector3.zero);

	// Use this for initialization
	void Start () {
        Cat = GameObject.FindWithTag("Player");
        cam = GameObject.FindWithTag("MainCamera");
        controller = GetComponent<CharacterController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        patrolPoints = new List<patrolBehavior>();
        patrolPoints.Add(new patrolBehavior(new Vector2(-1.5f, -1.5f), 1.0f));
        patrolPoints.Add(new patrolBehavior(new Vector2(-1.5f, -3.5f), 1.0f));
        patrolPoints.Add(new patrolBehavior(new Vector2(-2.5f, -3.5f), 1.0f));
        patrolPoints.Add(new patrolBehavior(new Vector2(-2.5f, -1.5f), 1.0f));
        GameObject sight = Instantiate(sightPrefab);
        sight.GetComponent<VisionScript>().scientist = gameObject;
    }

    // Update is called once per frame
    void Update () {
        transform.GetChild(0).rotation = cam.transform.rotation;
        if (chasing)
        {
            Chase();
        }
        else
        {
            Patrol();
        }
        if (controller.velocity.magnitude > 0.01f)
        {
            facingQuat = Quaternion.LookRotation(new Vector3(-controller.velocity.z, 0, controller.velocity.x));
        }
        if (Mathf.Abs(controller.velocity.x) < Mathf.Abs(controller.velocity.z) && controller.velocity.z > 0)
        {
            spriteRenderer.sprite = back;
            spriteRenderer.flipX = true;
        }
        else if (Mathf.Abs(controller.velocity.x) < Mathf.Abs(controller.velocity.z) && controller.velocity.z < 0)
        {
            spriteRenderer.sprite = front;
            spriteRenderer.flipX = false;
        }
        else if (Mathf.Abs(controller.velocity.x) > Mathf.Abs(controller.velocity.z) && controller.velocity.x > 0)
        {
            spriteRenderer.sprite = front;
            spriteRenderer.flipX = true;
        }
        else if (Mathf.Abs(controller.velocity.x) > Mathf.Abs(controller.velocity.z) && controller.velocity.x < 0)
        {
            spriteRenderer.sprite = back;
            spriteRenderer.flipX = false;
        }
    }

    void Chase()
    {
        controller.Move(new Vector3(Cat.transform.position.x - transform.position.x, 0, Cat.transform.position.z - transform.position.z).normalized * Time.deltaTime * speed);
    }

    void Patrol()
    {
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), patrolPoints[patrolIndex].destination) <= 0.1f)
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
