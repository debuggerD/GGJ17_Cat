using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

public class CatScript : MonoBehaviour {

    Rigidbody rigid;
    CharacterController controller;
	public GameObject CatPiece;
	public AudioSource QuantumSoundEffect;

	public AudioMixer DefaultMixer;
	public AudioMixerSnapshot DefaultBGM;
	public AudioMixerSnapshot QuantumBGM;

    bool isFirstFrame;

	IEnumerator fadeOut()
	{
		float t = QuantumSoundEffect.volume;
		while (t > 0.0f) {
			t -= Time.deltaTime;
			QuantumSoundEffect.volume = t;
			yield return new WaitForSeconds(0);
		}
		QuantumSoundEffect.volume = 0.0f;
		QuantumSoundEffect.Stop ();
	}


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
    public bool controllable = true;
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
        isFirstFrame = true;

        rigid = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        cam = GameObject.FindWithTag("MainCamera");

        // Initialize Logical Position
        transform.position = new Vector3(GameManagerScript.GetXPos(m_kCatLogicalStartI, m_kCatLogicalStartJ), 0.01f, GameManagerScript.GetZPos(m_kCatLogicalStartI, m_kCatLogicalStartJ));
        transform.GetChild(0).rotation = cam.transform.rotation;
        m_lastGridPosI = m_kCatLogicalStartI;
        m_lastGridPosJ = m_kCatLogicalStartJ;
        disintegration = FileToArray ("disintegration");
		disintegrated_positions = new float[disintegration.GetLength(0), 2];
		quantized_pieces = new GameObject[disintegration.GetLength(0)];

		manager = GameObject.Find ("GameManager");
        manager.GetComponent<GameManagerScript>().NotifyCatMove(m_lastGridPosI, m_lastGridPosJ, false);

		qvis = GameObject.Find ("QVisualizer");
    }

	GameObject manager = null;
	GameObject qvis = null;

    // Update is called once per frame
    Vector3 lastValidPosition;

	void StartQuantization()
	{
		for(int i = 0; i < disintegration.GetLength(0); i++) {
			var go = Instantiate(CatPiece);
			quantized_pieces[i] = go;
            go.transform.rotation = cam.transform.rotation;
            //go.transform.rotation = Quaternion.LookRotation(cam.transform.position - transform.position);
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
			p.y -= (disintegration [i, 1]-68.5f+0f) / scale*1.414f;
			go.transform.position = p;
			disintegrated_positions [i, 0] = (float)disintegration [i, 0];
			disintegrated_positions [i, 1] = (float)disintegration [i, 1];
		}
		qvis.GetComponent<QScript> ().active = true;
		qvis.transform.position = transform.position;

		QuantumSoundEffect.volume = 1f;
		QuantumSoundEffect.Play();
		QuantumBGM.TransitionTo(2f);

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
		p.x = pos.x;
		p.z = pos.y;
		var gms = manager.GetComponent<GameManagerScript> ();
		gms.warp_count += 1;
		gms.NotifyCatMove(GameManagerScript.GetGridIPos(p.x, p.z), GameManagerScript.GetGridJPos(p.x, p.z), false);
		transform.position = p;

		qvis.GetComponent<QScript> ().Clear ();



		StartCoroutine(fadeOut());
		DefaultBGM.TransitionTo(2f);

	}

    void Update() {
        transform.GetChild(0).rotation = cam.transform.rotation;

        // Move by WASD
        if (!quantized & controllable)
        {
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

                GameObject.Find("GameManager").GetComponent<GameManagerScript>().NotifyCatMove(m_lastGridPosI, m_lastGridPosJ, true);
            }
        }
        else if (isFirstFrame)
        {
            m_lastGridPosI = GameManagerScript.GetGridIPos(transform.position.x, transform.position.z);
            m_lastGridPosJ = GameManagerScript.GetGridJPos(transform.position.x, transform.position.z);

            GameObject.Find("GameManager").GetComponent<GameManagerScript>().NotifyCatMove(m_lastGridPosI, m_lastGridPosJ, true);
        }

		bool force_q = false;
		if (quantized && !qvis.GetComponent<QScript> ().InCatomCloudForm ()) {
			force_q = true;

			manager.GetComponent<GameManagerScript> ().GetAchivement (GameManagerScript.Achivements.HeisenburgUncatiantyPrinciple);
		}
		if (force_q || Input.GetKeyDown ("q")) {
			
			bool on_sight = false;
			if (!quantized) {
				// if cat is inside vision range, cant quantize
				var seeing = manager.GetComponent<GameManagerScript>().seeing;
				var gi = (int)(transform.position.z / 0.25f);
				var gj = (int)(transform.position.x / 0.25f);
				if (seeing [gi, gj] > 0)
					on_sight = true;
			}

			if (!on_sight)
			{
                GetComponentInChildren<SpriteRenderer> ().enabled = !GetComponentInChildren<SpriteRenderer> ().enabled;
                quantized = !GetComponentInChildren<SpriteRenderer> ().enabled;
				if (quantized) {
					StartQuantization ();
				} else {
					StopQuantization ();
				}
			}
		}
        //if (Input.GetKeyDown("space"))
        //{
        //    controller.Move(new Vector3(Random.Range(-2.0f, 2.0f), 0, Random.Range(-2.0f, 2.0f)));
        //}
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
				p.y -= (y-68.5f+0f) / scale*1.414f;
				var go = quantized_pieces [i];
				go.transform.position = p;
			}
		}
    }

    void FixedUpdate()
    {

    }


    protected Vector3 default_rotation = new Vector3(26.57f, -45, 0);

    //////////////////////////////////////////////////////////////////////////////// 임시 코드데이터
    protected const int m_kCatLogicalStartI = 3;
    protected const int m_kCatLogicalStartJ = 52;//92;

   
}
