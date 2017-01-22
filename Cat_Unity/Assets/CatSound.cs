using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CatSound : MonoBehaviour {

	public AudioClip[] clips;
	public AudioMixerGroup MixerGroup;
	private int clipIndex;
	private AudioSource catAudio;
	public float CatAudioMinDistance;
	public float CatAudioMaxDistance;
	public float CatAudioVolume;

	// Use this for initialization
	void Start () {
		catAudio = gameObject.AddComponent<AudioSource>();
		catAudio.outputAudioMixerGroup = MixerGroup;
		catAudio.spatialBlend = 1.0f;
		catAudio.minDistance = CatAudioMinDistance;
		catAudio.maxDistance = CatAudioMaxDistance;
		catAudio.volume = CatAudioVolume;
		catAudio.rolloffMode = AudioRolloffMode.Linear;
	}
	
	// Update is called once per frame
	void Update () {
		if (!catAudio.isPlaying) 
		{
			clipIndex = Random.Range(0, clips.Length - 1);
			catAudio.clip = clips[clipIndex];
			catAudio.PlayDelayed(Random.Range(2f, 5f));
			// Debug.Log("Nothing playing, we set new audio to " + audio.clip.name);
		}
	}
}
