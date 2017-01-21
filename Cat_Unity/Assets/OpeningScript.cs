using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		frames = new Texture[] {
			Resources.Load("story1") as Texture,
			Resources.Load("story2") as Texture,
			Resources.Load("story3") as Texture,
		};
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	Texture[] frames;
	float frameTime = 5.0f;
	private int frame=-1;
	private float nextFrameTime = 0.0f;
	float baseTime = 0.0f;
	void OnGUI()
	{
		if (frame < frames.Length) {
			if (Time.time >= nextFrameTime) {
				frame++;
				baseTime = Time.time;
				nextFrameTime += frameTime;
				GUI.color = new Color (1f, 1f, 1f, 1f);
			}
			if (frame == frames.Length) {
				SceneManager.LoadScene ("GameScene_js");
			} else {
				var dt = Time.time - baseTime;
				if (dt < 1)
					GUI.color = new Color (1f, 1f, 1f, dt);
				else if (dt < 4)
					GUI.color = new Color (1f, 1f, 1f, 1f);
				else if (dt < 5)
					GUI.color = new Color (1f, 1f, 1f, 5 - dt);
				GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), frames [frame]);
			}
		}
	}
}
