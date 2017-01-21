using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionScript : MonoBehaviour {
	public const int SightAngle = 60;
	public const int SightLength = 4;
	public Vector3[] vertices;
	public Vector2[] newUV;
	public int[] triangles;
    GameObject scientist;

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
        scientist = GameObject.Find("Scientist");
    }

	// Update is called once per frame
	void Update () {
        transform.position = scientist.transform.position;
		var rot = transform.rotation;
		var delta = Input.mousePosition.x;
		rot.eulerAngles = new Vector3 (0.0f, delta, 0.0f);
		for (var i = 0; i < 60; i++) {
			Ray ray = new Ray (transform.position,
				new Vector3 (Mathf.Cos ((i -delta- 30) / 180f * 3.141592f), 0.01f, Mathf.Sin ((i -delta- 30) / 180f * 3.141592f)));
			RaycastHit hit;
			float len = SightLength;
			if (Physics.Raycast (ray, out hit, SightLength)) {
				len = hit.distance;
			}
			vertices[i] = new Vector3(len*Mathf.Cos((i-30)/180.0f*3.141592f), 0.01f, len*Mathf.Sin((i-30)/180.0f*3.141592f));
		}
		GetComponent<MeshFilter> ().mesh.vertices = vertices;
		//rot.z = Input.mousePosition.x;
		transform.rotation = rot;
	}
}