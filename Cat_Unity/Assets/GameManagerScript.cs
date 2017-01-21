using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManagerScript : MonoBehaviour {

    public GameObject Tile;
    public GameObject Wall;
    int width = 0;
    int length = 0;

    int[,] map_move;
    int[,] map_height;
    int[,] map_wave_pass;
    int[,] map_wave_modify;
    int[,] map_wall;

    // Use this for initialization
    void Start () {
        GetMetaData();
        ReadMapData();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void GetMetaData()
    {
        //string text = System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt");
        //string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Public\TestFolder\WriteLines2.txt");
        //foreach (string line in lines)
        //{
        //    Console.WriteLine("\t" + line);
        //}

        int[,] result = new int[width, length];
        TextAsset txtFile = (TextAsset)Resources.Load("map_meta") as TextAsset;
        print(txtFile);
        string[] textArray = txtFile.text.Trim().Split('\t');
        width = Int32.Parse(textArray[0]);
        length = Int32.Parse(textArray[1]);
        print(width);
        print(length);
    }

    int[,] FileToArray(string filename)
    {
        int[,] result = new int[width, length];
        TextAsset txtFile = (TextAsset)Resources.Load(filename) as TextAsset;
        string[] lines = txtFile.text.Trim().Split('\n');
        for (int i = 0; i < length; i++)
        {
            string[] line = lines[i].Split('\t');
            for (int j = 0; j < width; j++)
            {
                result[j, i] = Int32.Parse(line[j]);
            }
        }
        return result;
    }

    void ReadMapData()
    {
        map_move = FileToArray("map_move_m");
        map_height = FileToArray("map_height_m");
        map_wave_pass = FileToArray("map_wave_pass_m");
        map_wave_modify = FileToArray("map_wave_modify_m");
        map_wall = FileToArray("map_wall_m");
        CreateMap();
    }

    void CreateMap()
    {
        // Tiling
        const float halfTileLength = 0.5f;
        for (int i = 0; i < width; i += 2)
        {
            for (int j = 0; j < length; j += 2)
            {
                GameObject createdTile = Instantiate(Tile);
                createdTile.transform.position = new Vector3(GetXPos(i, j) + halfTileLength, 0, GetZPos(i, j) + halfTileLength);
            }
        }

        // Make Walls
        const float halfWallLength = m_kUnitLength * 0.5f;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                GameObject createdWallPatch = null;
                switch (map_wall[i,j])
                {
                    case 0:
                        createdWallPatch = Instantiate(Wall);
                        createdWallPatch.transform.position = new Vector3(GetXPos(i, j) + halfWallLength, 0, GetZPos(i, j) + halfWallLength);
                        break;

                    case 2:
                        createdWallPatch = Instantiate(Wall);
                        createdWallPatch.transform.position = new Vector3(GetXPos(i, j) + halfWallLength, 0, GetZPos(i, j) + halfWallLength);
                        break;
                }
                if (createdWallPatch != null)
                {
                    ApplyNormalWallTransform(createdWallPatch);
                }
            }
        }
    }

    public static float GetXPos(float gridI, float gridJ)
    {
        return gridJ * m_kUnitLength;
    }

    public static float GetZPos(float gridI, float gridJ)
    {
        return gridI * m_kUnitLength;
    }

    protected void ApplyNormalWallTransform(GameObject wallObj)
    {
        wallObj.transform.localScale = new Vector3(wallObj.transform.localScale.x, m_kWallHeight, wallObj.transform.localScale.z);
        wallObj.transform.position = new Vector3(wallObj.transform.position.x, m_kWallHeight * 0.5f, wallObj.transform.position.z);
    }

    //////////////////////////////////////////////////////////////////////////////// 임시 코드데이터
    protected const float m_kUnitLength = 0.5f;
    protected const float m_kWallHeight = 1.5f;
}
