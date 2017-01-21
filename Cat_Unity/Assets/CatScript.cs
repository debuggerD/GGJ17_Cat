using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CatScript : MonoBehaviour {

    Rigidbody rigid;
    CharacterController controller;
	public GameObject CatPiece;

    protected float m_lastGridPosI;
    public float GridPosI
    {
        get
        {
            return m_lastGridPosI;
        }
    }

    protected float m_lastGridPosJ;
    public float GridPosJ
    {
        get
        {
            return m_lastGridPosJ;
        }
    }

    bool quantized = false;
	int [,] disintegration;
	float [,] disintegrated_positions;
	GameObject[] quantized_pieces;

    float speed = 2.0f;
    GameObject cam;

	int [,] FileToArray(string filename) {
		int[,] result = null;
		bool first = true;
		TextAsset txtFile = (TextAsset)Resources.Load(filename) as TextAsset;
		string[] lines = txtFile.text.Trim().Split('\n');
		for (int i = 0; i < lines.Length; i++)
		{
			string[] line = lines[i].Trim().Split('\t');
			if (first) {
				result = new int[lines.Length,line.Length]; 
				first = false;
			}
			for (int j = 0; j < line.Length; j++)
			{
				result[i, j] = System.Int32.Parse(line[j]);
			}
		}
		return result;
	}

	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        cam = GameObject.FindWithTag("MainCamera");

        // Initialize Logical Position
        transform.position = new Vector3(GameManagerScript.GetXPos(m_kCatLogicalStartI, m_kCatLogicalStartJ), 0.5f, GameManagerScript.GetZPos(m_kCatLogicalStartI, m_kCatLogicalStartJ));
        transform.rotation = cam.transform.rotation;
        m_lastGridPosI = m_kCatLogicalStartI;
        m_lastGridPosJ = m_kCatLogicalStartJ;
        disintegration = FileToArray ("disintegration");
		disintegrated_positions = new float[disintegration.GetLength(0), 2];
		quantized_pieces = new GameObject[disintegration.GetLength(0)];

		qvis = GameObject.Find ("QVisualizer");
    }

	GameObject qvis = null;

    // Update is called once per frame
    Vector3 lastValidPosition;

	void StartQuantization()
	{
		for(int i = 0; i < disintegration.GetLength(0); i++) {
			var go = Instantiate(CatPiece);
			quantized_pieces[i] = go;
            go.transform.rotation = Quaternion.LookRotation(cam.transform.position - transform.position);
			//go.transform.eulerAngles = new Vector3 (30f, 45f, 0.0f);
			go.GetComponent<SpriteRenderer>().color = new Color(
				disintegration[i,2]/255f,
				disintegration[i,3]/255f,
				disintegration[i,4]/255f,
				disintegration[i,5]/255f
			);
			var p = this.transform.position;
			const float scale = 130f;
			p.x -= (disintegration [i, 0]-57f+30f) / scale;
			p.z -= (disintegration [i, 0]-57f+30f) / scale;
			p.y -= (disintegration [i, 1]-68.5f+40f) / scale*1.414f;
			go.transform.position = p;
			disintegrated_positions [i, 0] = (float)disintegration [i, 0];
			disintegrated_positions [i, 1] = (float)disintegration [i, 1];
		}
		qvis.GetComponent<QScript> ().active = true;
		qvis.transform.position = transform.position;
	}

	void StopQuantization()
	{
		foreach (var go in quantized_pieces) {
			Destroy (go);
		}
		for (var i = 0; i < quantized_pieces.Length; i++) {
			quantized_pieces [i] = null;
		}

		// do warp!
		var pos = qvis.GetComponent<QScript> ().PickPosition ();
		var p = transform.position;
		print (pos.x);
		print( pos.y);
		print (p.x);
		print( p.z);
		p.x = pos.x;
		p.z = pos.y;
		transform.position = p;

		qvis.GetComponent<QScript> ().Clear ();
	}

    void Update() {
        Vector3 relativePos = cam.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = rotation;

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
            
            m_lastGridPosI = GameManagerScript.GetGridIPos(transform.position.x, transform.position.z);
            m_lastGridPosJ = GameManagerScript.GetGridJPos(transform.position.x, transform.position.z);
        }

		if (Input.GetKeyDown ("q")) {
			GetComponent<SpriteRenderer> ().enabled = !GetComponent<SpriteRenderer> ().enabled;
			quantized = !GetComponent<SpriteRenderer> ().enabled;
			if (quantized) {
				StartQuantization ();
			} else {
				StopQuantization ();
			}
		}
        if (Input.GetKeyDown("space"))
        {
            controller.Move(new Vector3(Random.Range(-2.0f, 2.0f), 0, Random.Range(-2.0f, 2.0f)));
        }
		if (quantized) {
			for (var i = 0; i < disintegration.GetLength (0); i++) {
				var x = disintegrated_positions [i, 0];
				var y = disintegrated_positions [i, 1];
				float r = 120;
				float theta = Time.time + i / 2f;
				float tx = r * Mathf.Cos (theta) + 100;
				float ty = r * Mathf.Sin (2 * theta) / 2 + 100-70;
				tx /= 2;
				ty /= 2;
				const float sr = 4f;
				tx += sr * Mathf.Cos (i/2f);
				ty += sr * Mathf.Sin (i/2f);

				float dx = tx - x;
				float dy = ty - y;
				float l = Mathf.Sqrt (dx * dx + dy * dy);
				if (l > 0.001) {
					const float speed_limit = 1.1f;
					//if to_cat:
					//if l < speed_limit:
					//	l = speed_limit
					x += dx / l * speed_limit;
					y += dy / l * speed_limit;
					//did_move += 1
				}
				var p = this.transform.position;
				const float scale = 130f;
				disintegrated_positions [i, 0] = x;
				disintegrated_positions [i, 1] = y;
				p.x -= (x-57f+30f) / scale;
				p.z -= (x-57f+30f) / scale;
				p.y -= (y-68.5f+40f) / scale*1.414f;
				var go = quantized_pieces [i];
				go.transform.position = p;
			}
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

    protected Vector3 default_rotation = new Vector3(26.57f, -45, 0);

    //////////////////////////////////////////////////////////////////////////////// 임시 코드데이터
    protected const int m_kCatLogicalStartI = 2;
    protected const int m_kCatLogicalStartJ = 19;

   
}
