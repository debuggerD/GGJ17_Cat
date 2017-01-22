using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScript : MonoBehaviour {
	public Button fireButton;

	// Use this for initialization
	void Start () {
		fireButton.onClick.AddListener(StartGame);
	}

	IEnumerator DoStart() {
		yield return new WaitForSeconds(2.4f);
		UnityEngine.SceneManagement.SceneManager.LoadScene ("OpeningScene");
	}

	void StartGame() {
		GetComponent<AudioSource> ().Play ();

		StartCoroutine (DoStart());
	}
	
	// Update is called once per frame
	void Update () {
	}
}
