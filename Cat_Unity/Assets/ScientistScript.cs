using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    GameObject manager;
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
    bool catched = false;
    float remainTime = 1;
    int patrolIndex = 0;
    public Quaternion facingQuat = Quaternion.Euler(Vector3.zero);

	// Use this for initialization
	void Start () {
        Cat = GameObject.FindWithTag("Player");
        cam = GameObject.FindWithTag("MainCamera");
        manager = GameObject.Find("GameManager");
        controller = GetComponent<CharacterController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        patrolPoints = new List<patrolBehavior>();
        patrolPoints.Add(new patrolBehavior(new Vector2(46.5f, 6.0f), 1.0f));
        patrolPoints.Add(new patrolBehavior(new Vector2(37f, 6.0f), 1.0f));

        GameObject sight = Instantiate(sightPrefab);
        sight.GetComponent<VisionScript>().scientist = gameObject;
        Physics.IgnoreLayerCollision(8, 9, true);
    }
    float cageTime = 0;
    // Update is called once per frame
    void Update () {
        if (Vector2.Distance(new Vector2(Cat.transform.position.x, Cat.transform.position.z), new Vector2(transform.position.x, transform.position.z)) < 1.0f && !catched)
        {
            manager.GetComponent<GameManagerScript>().GetAchivement(GameManagerScript.Achivements.TimeTravel);
            catched = true;
            Cat.GetComponent<CatScript>().controllable = false;
            GetComponent<CharacterController>().enabled = false;
            Cat.GetComponent<CharacterController>().enabled = false;
            spriteRenderer.sprite = front;
            spriteRenderer.flipX = true;
        }
        if (catched)
        {
            GameObject cage = GameObject.FindWithTag("Cage");
            if (Vector3.Distance(transform.position, cage.transform.position) < 3.0f)
            {
                if (cageTime < 2.0f)
                {
                    cage.transform.position = Vector3.MoveTowards(cage.transform.position, cage.transform.position + new Vector3(0, 0.1f, 0), 0.5f * Time.deltaTime);
                }
                else if (cageTime < 4.0f)
                {
                    Cat.transform.position = Vector3.MoveTowards(Cat.transform.position, new Vector3(cage.transform.position.x, 0.2f, cage.transform.position.z), 1.0f * Time.deltaTime);
                }
                else if (cageTime < 6.0f)
                {
                    cage.transform.position = Vector3.MoveTowards(cage.transform.position, cage.transform.position + new Vector3(0, -0.1f, 0), 0.5f * Time.deltaTime);
                }
                else
                {
                    SceneManager.LoadScene("GameScene_js");
                }
                cageTime += Time.deltaTime;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(cage.transform.position.x, 0.8f, cage.transform.position.z), 1.0f * Time.deltaTime);
                Cat.transform.position = transform.position + new Vector3(.5f, 0, -.5f);
            }
        }
        transform.GetChild(0).rotation = cam.transform.rotation;
        if (chasing)
        {
            Chase();
        }
        else if (!catched)
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

    void ReturnCat()
    {
        
    }
}
