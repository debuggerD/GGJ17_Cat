using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningScript : MonoBehaviour {

	public AudioClip openingBGMclip;
	private AudioSource openingBGM;

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
	void OnGUI()
	{
		if (frame < frames.Length) {
			if (Time.time >= nextFrameTime) {
				frame++;
				baseTime = Time.time;
				nextFrameTime += frameTime;
				GUI.color = new Color (1f, 1f, 1f, 1f);
				sound_played = false;
			}
			if (frame == frames.Length) {
				SceneManager.LoadScene ("GameScene_js");
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
			}
		}
	}
}
