using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QScript : MonoBehaviour {
	Dictionary<KeyValuePair<int,int>, int> energy;

	public Vector3[] vertices;
	public int[] triangles;
	public bool active = false;

	GameObject manager;

	// Use this for initialization
	void Start () {
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter> ().mesh = mesh;
		Clear ();

		manager = GameObject.Find ("GameManager");
	}
	const float height = 0.001f;
	public void Clear()
	{
		energy = new Dictionary<KeyValuePair<int,int>,int>();
		energy[new KeyValuePair<int,int>(0,0)] = 10000;
		active = false;

		GetComponent<MeshFilter> ().mesh.vertices = vertices = new Vector3[]{};
		GetComponent<MeshFilter> ().mesh.triangles = triangles = new int[]{};
	}

	int[][] directions = new int[][] {new int[]{-1,0,100},new int[]{1,0,100},new int[]{0,1,100}, new int[]{0,-1,100}
		,new int[]{-1,-1,141}, new int[]{1,1,141}, new int[]{-1,1,141}, new int[]{1,-1,141}};

	int GetFlow(int f, int t)
	{
		int d = f - t;
		if (d < 10)
			return 0;
		//if (d > 1000)
		//  d = 1000;
		return 1 + (int)Mathf.Sqrt (d-1)/10;
	}
	// Update is called once per frame
	int sub_tx;
	int sub_ty;
	public Vector2 PickPosition()
	{
		int pick = Random.Range (0, 10000);
		foreach(var kv in energy) {
			pick -= kv.Value;
			if (pick < 0){
				return new Vector2((kv.Key.Key+sub_tx)/4f+0.25f, (kv.Key.Value+sub_ty)/4f+0.25f);
			}
		}
		return Vector2.zero;
	}

	void Update () {
		if (!active)
			return;
		int[,] pass = manager.GetComponent<GameManagerScript> ().map_wave_pass;
		//int[,] mod  = manager.GetComponent<GameManagerScript> ().map_wave_modify;
		const float scale = 1.0f/4;
		const int sub_tile_count = 2;
		//int tx = Mathf.Floor(this.transform.z / 0.5f);
		//int ty = Mathf.Floor(this.transform.x / 0.5f);
		sub_tx = (int)Mathf.Floor(this.transform.position.x / scale);
		sub_ty = (int)Mathf.Floor(this.transform.position.z / scale);

		System.Func<int, int, bool> Passable = (int x, int y) => {
			var tx = (sub_tx + x) / sub_tile_count;
			var ty = (sub_ty + y) / sub_tile_count;
			if (tx < 0 || ty < 0 || tx >= pass.GetLength(1) || ty >= pass.GetLength(0))
				return false;
			return pass[ty,tx] > 0;
		};

		Dictionary<KeyValuePair<int,int>, int> next = new Dictionary<KeyValuePair<int,int>,int>();
		foreach (var kv in energy) {
			var k = kv.Key;
			var v = kv.Value;
			var delta = 0;
			foreach (var dir in directions) {
				KeyValuePair<int,int> nk = new KeyValuePair<int,int>(k.Key + dir[0], k.Value + dir[1]);
				if (!Passable (nk.Key, nk.Value))
					continue;
				int nv = 0;
				bool has = false;
				if (energy.ContainsKey (nk)) {
					nv = energy [nk];
					has = true;
				}
				if (v > nv) {
					int f = GetFlow (v, nv) * 100 / dir [2];
					delta -= f;
					if (!has) {
						if (next.ContainsKey (nk)) {
							next [nk] += f;
						} else {
							next [nk] = f;
						}
					}
				} else if (v < nv) {
					delta += GetFlow (nv, v) * 100 / dir[2] ;
				}
			}
			next[k] = v + delta;
		}
		energy = next;
		var minx = 0;
		var maxx = 0;
		var miny = 0;
		var maxy = 0;
		foreach (var kv in energy) {
			var k = kv.Key;
			var x = k.Key;
			var y = k.Value;
			if (x > maxx)
				maxx = x;
			if (x < minx)
				minx = x;
			if (y > maxy)
				maxy = y;
			if (y < miny)
				miny = y;
		}
		int idx = 0;
		for (int x = minx; x <= maxx; x++) {
			int y = miny;
			while (y <= maxy) {
				while (y <= maxy && !energy.ContainsKey (new KeyValuePair<int,int>(x, y)))
					y++;
				if (y > maxy)
					break;
				//int by = y;
				while (y <= maxy && energy.ContainsKey (new KeyValuePair<int,int>(x, y)))
					y++;
				//int ey = y;
				idx ++;
			}
		}

		vertices = new Vector3[idx*4];
		triangles = new int[idx * 6];
		idx = 0;
		for (int x = minx; x <= maxx; x++) {
			int y = miny;
			while (y <= maxy) {
				while (y <= maxy && !energy.ContainsKey (new KeyValuePair<int,int>(x, y)))
					y++;
				if (y > maxy)
					break;
				int by = y;
				while (y <= maxy && energy.ContainsKey (new KeyValuePair<int,int>(x, y)))
					y++;
				int ey = y;
				vertices[idx*4] = new Vector3(x*scale, 0, by*scale);
				vertices[idx*4+1] = new Vector3((x+1)*scale, 0, by*scale);
				vertices[idx*4+2] = new Vector3(x*scale, 0, ey*scale);
				vertices[idx*4+3] = new Vector3((x+1)*scale,0,ey*scale);

				triangles [idx * 6] = idx * 4;
				triangles [idx * 6+1] = idx * 4+2;
				triangles [idx * 6+2] = idx * 4+1;
				triangles [idx * 6+3] = idx * 4+1;
				triangles [idx * 6+4] = idx * 4+2;
				triangles [idx * 6+5] = idx * 4+3;
				idx += 1;
			}
		}

		/*foreach (var kv in energy) {
            var k = kv.Key;
            var x = k.Key;
            var y = k.Value;
            vertices[idx*4] = new Vector3(x*scale, 0, y*scale);
            vertices[idx*4+1] = new Vector3((x+1)*scale, 0, y*scale);
            vertices[idx*4+2] = new Vector3(x*scale, 0, (y+1)*scale);
            vertices[idx*4+3] = new Vector3((x+1)*scale,0,(y+1)*scale);

            triangles [idx * 6] = idx * 4;
            triangles [idx * 6+1] = idx * 4+2;
            triangles [idx * 6+2] = idx * 4+1;
            triangles [idx * 6+3] = idx * 4+1;
            triangles [idx * 6+4] = idx * 4+2;
            triangles [idx * 6+5] = idx * 4+3;
            idx += 1;
        }*/
		GetComponent<MeshFilter> ().mesh.vertices = vertices;
		GetComponent<MeshFilter> ().mesh.triangles = triangles;
	}
}