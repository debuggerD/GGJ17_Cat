using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManagerScript : MonoBehaviour {

    public GameObject Tile;
    public GameObject Wall;
    int[,] tiles;
	// Use this for initialization
	void Start () {
        ReadMapData();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    int[,] FileToArray(string filename)
    {
        TextAsset txtFile = (TextAsset)Resources.Load(filename) as TextAsset;
        string[] lines = txtFile.text.Trim().Split('\n');
        return null;
        //still writing
    }

    void ReadMapData()
    {
        int worldWidth = 10;
        int worldLength = 10;
        string Filename = "test";
        TextAsset txtFile = (TextAsset)Resources.Load(Filename) as TextAsset;
        string[] lines = txtFile.text.Trim().Split('\n');
        worldWidth = lines.Length;
        worldLength = lines[0].Split(',').Length;
        tiles = new int[worldWidth, worldLength];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(',');
            for (int j = 0; j < line.Length; j++)
            {
                tiles[i, j] = Int32.Parse(line[j]);
                //print(tiles[i, j]);
            }
        }
        CreateMap();
    }

    void CreateMap()
    {
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                GameObject tile = Instantiate(Tile);
                tile.transform.position = new Vector3(i * 1, 0, j * 1);
            }
        }
    }
}
