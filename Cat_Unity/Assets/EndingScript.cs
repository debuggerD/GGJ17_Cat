using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		frame = Resources.Load ("ending") as Texture;
		start_time = Time.time;
	}

	Texture frame;
	float start_time;
	
	// Update is called once per frame
	void OnGUI () {
		var dt = Time.time - start_time;
		if (dt > 3 && Input.anyKey || dt > 10) {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("TitleScene");
		}
		if (dt < 1) {
			GUI.color = new Color (1f, 1f, 1f, dt);
		} else {
			GUI.color = new Color (1f, 1f, 1f, 1f);
		}
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), frame);
	}
}
