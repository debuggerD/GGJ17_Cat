﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionScript : MonoBehaviour {
	public const int SightAngle = 60;
	public const int SightLength = 4;
	public Vector3[] vertices;
	public Vector2[] newUV;
	public int[] triangles;
    public GameObject scientist;
	GameObject manager;

	// Use this for initialization
	void Start () {
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		vertices = new Vector3[61];
		for (var i = 0; i < 60; i++) {
			vertices[i] = new Vector3(SightLength*Mathf.Cos((i-30)/180.0f*3.141592f), 0.01f, SightLength*Mathf.Sin((i-30)/180.0f*3.141592f));
		}
		vertices [60] = new Vector3 (0.0f, 0.01f, 0.0f);
		mesh.vertices = vertices;
		//mesh.uv = newUV;
		triangles = new int[60*3];
		for (var i = 0; i < 60; i++) {
			triangles [i * 3] = 60;
			triangles [i * 3 + 1] = i+1;
			triangles [i * 3 + 2] = i;
		}
		mesh.triangles = triangles;
		manager = GameObject.Find ("GameManager");
    }

	// Update is called once per frame
	void Update () {
        transform.position = scientist.transform.position + new Vector3(0, -0.6f, 0);
		var seeing = manager.GetComponent<GameManagerScript> ().seeing;
		var rot = scientist.GetComponent<ScientistScript>().facingQuat;
        var delta = scientist.GetComponent<ScientistScript>().facingQuat.eulerAngles.y;
        rot.eulerAngles = new Vector3 (0.0f, delta, 0.0f);
		for (var i = 0; i < 60; i++) {
			Ray ray = new Ray (transform.position,
				new Vector3 (Mathf.Cos ((i -delta- 30) / 180f * 3.141592f), 0.01f, Mathf.Sin ((i -delta- 30) / 180f * 3.141592f)));
			RaycastHit hit;
			float len = SightLength;
			if (Physics.Raycast (ray, out hit, SightLength, ~((1<<8) | (1<<10)))) {
				len = hit.distance;
			}
			for(var j = 1; j <= 40;j++)
			{
				Vector3 p = transform.position;
				p += new Vector3 (Mathf.Cos ((i -delta- 30) / 180f * 3.141592f), 0.01f, Mathf.Sin ((i -delta- 30) / 180f * 3.141592f))*(1f*j/40)*len;

				int gi = (int)(p.z / 0.25f);
				int gj = (int)(p.x / 0.25f);
				if (gi >= 0 && gj >= 0 && gi < seeing.GetLength (0) && gj < seeing.GetLength (1)) {
					seeing [gi, gj] = 1;
				}
			}
			vertices[i] = new Vector3(len*Mathf.Cos((i-30)/180.0f*3.141592f), 0.01f, len*Mathf.Sin((i-30)/180.0f*3.141592f));
		}
		GetComponent<MeshFilter> ().mesh.vertices = vertices;
        //rot.z = Input.mousePosition.x;
        //transform.rotation = rot;

        transform.rotation = scientist.GetComponent<ScientistScript>().facingQuat;
	}
}