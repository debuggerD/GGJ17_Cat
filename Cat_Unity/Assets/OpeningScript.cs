using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpeningScript : MonoBehaviour {

	public AudioClip openingBGMclip;
	private AudioSource openingBGM;
	public Text subscription;

	// Use this for initialization
	void Start () {
		frames = new Texture[] {
			Resources.Load("story1") as Texture,
			Resources.Load("story2") as Texture,
			Resources.Load("story3") as Texture,
		};
		clips = new AudioClip[] {
			Resources.Load("s1",typeof(AudioClip)) as AudioClip,
			Resources.Load("s2",typeof(AudioClip)) as AudioClip,
			Resources.Load("s3",typeof(AudioClip)) as AudioClip,
		};
		openingBGM = gameObject.AddComponent<AudioSource>();
		openingBGM.clip = openingBGMclip;
		openingBGM.volume = 0.1f;
		openingBGM.loop = true;
		openingBGM.Play();
		baseTime = Time.time;
		nextFrameTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	Texture[] frames;
	AudioClip[] clips;
	float frameTime = 18.0f;
	private int frame=-1;
	private float nextFrameTime = 0.0f;
	float baseTime = 0.0f;
	bool sound_played = false;
	string[] subs = new string[] {
		"As you already know, cats are made up of catom\n" +
		"                       (which means cat+atom).\nScientists like Schrödinger try to research catom and\n    you know the Schrödinger's cat experiment.",
		"A cat named Tom watched its friend becoming both alive and dead,\n          or in a quantum superposition.\nTom is scared. Tom wants to run away from here.",
		"Due to unfortunate accident, Tom is showered by a W-ray.\nNow Tom is able to activate it's catom's wave function and\n     become a WAVE!"
	};
	private Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	void OnGUI()
	{
		if (frame < frames.Length) {
			if (Time.time >= nextFrameTime) {
				frame++;
				baseTime = Time.time;
				nextFrameTime = baseTime + frameTime;
				GUI.color = new Color (1f, 1f, 1f, 1f);
				sound_played = false;
			}
			if (frame == frames.Length) {
				SceneManager.LoadScene ("GameScene_ys");
			} else {
				var dt = Time.time - baseTime;
				if (dt < 1)
					GUI.color = new Color (1f, 1f, 1f, dt);
				else if (dt < 17) {
					GUI.color = new Color (1f, 1f, 1f, 1f);
					if (!sound_played) {
						sound_played = true;
						var audio = GetComponent<AudioSource>(); 
						audio.PlayOneShot (clips [frame]);
						//subscription.text = subs [frame];
					}
					if (Input.anyKey){
						var audio = GetComponent<AudioSource>(); 
						audio.Stop ();
						baseTime = Time.time - 17;
						nextFrameTime = baseTime + 1;
					}
				}
				else if (dt < 18)
					GUI.color = new Color (1f, 1f, 1f, 18 - dt);
				GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), frames [frame]);
				GUIStyle gs = new GUIStyle();
				gs.fontSize = (int)(Screen.height * 0.05f);
				gs.normal.textColor = new Color (1f, 1f, 1f);
				gs.normal.background = MakeTex(2, 2,new Color (0f, 0f, 0f, 0.7f));
				//gs.fontStyle 
				var y = Screen.height * 0.7f;
				//if (frame != 1)
					y = Screen.height * 0.05f;
				//if (frame == 0)
					gs.normal.textColor = new Color (1f, 1f, 0f);
				GUI.Box (new Rect (0.05f*Screen.width, y, 0.9f*Screen.width, Screen.height * 0.25f), subs [frame], gs);
			}
		}
	}
}
