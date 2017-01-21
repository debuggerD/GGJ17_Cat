using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManagerScript : MonoBehaviour {

    public GameObject Tile;

    public GameObject NewWall;
    public GameObject WallCollider;

    int width = 0;
    int length = 0;

    int[,] map_move;
    int[,] map_height;
    int[,] map_wave_pass;
    int[,] map_wave_modify;
    int[,] map_wall;

    protected GameObject[,] m_wallObjects;

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
        _CreatTileObjects();
        _CreateWallObjects();        
    }


    #region Common Logic Interface

    public static float GetXPos(float gridI, float gridJ)
    {
        return gridJ * m_kUnitLength;
    }

    public static float GetZPos(float gridI, float gridJ)
    {
        return gridI * m_kUnitLength;
    }

    public static float GetGridIPos(float x, float z)
    {
        return z / m_kUnitLength;
    }

    public static float GetGridJPos(float x, float z)
    {
        return x / m_kUnitLength;
    }

    #endregion


    #region Common Logic

    #endregion


    #region Tile

    protected void _CreatTileObjects()
    {
        const float halfTileLength = 0.5f;
        for (int i = 0; i < width; i += 2)
        {
            for (int j = 0; j < length; j += 2)
            {
                GameObject createdTile = Instantiate(Tile);
                createdTile.transform.position = new Vector3(GetXPos(i, j) + halfTileLength, 0, GetZPos(i, j) + halfTileLength);
            }
        }
    }

    #endregion


    #region Wall

    protected void _CreateWallObjects()
    {
        m_wallObjects = new GameObject[width, length];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                GameObject createdWallPatch = null;
                switch (map_wall[i, j])
                {
                    case 0:
                        createdWallPatch = Instantiate(NewWall);
                        break;

                    case 2:
                        createdWallPatch = Instantiate(NewWall);
                        break;
                }
                if (createdWallPatch != null)
                {
                    ApplyNormalWallTransform(createdWallPatch, i, j);

                    WallUpdater wallDrawUpdater = createdWallPatch.GetComponent<WallUpdater>();
                    if (wallDrawUpdater != null)
                    {
                        wallDrawUpdater.gridI = i;
                        wallDrawUpdater.gridJ = j;
                    }

                    GameObject createdCollider = Instantiate(WallCollider);
                    createdCollider.transform.position = new Vector3(GetXPos(i, j), 0, GetZPos(i, j));
                    createdCollider.transform.localScale = new Vector3(m_kUnitLength, m_kWallHeight, m_kUnitLength);
                }
                m_wallObjects[i, j] = createdWallPatch;
            }
        }
    }

    #endregion

    protected void ApplyNormalWallTransform(GameObject wallObj, int i, int j)
    {
        wallObj.transform.position = new Vector3(GetXPos(i, j + 1), 0, GetZPos(i, j + 1));
    }

    //////////////////////////////////////////////////////////////////////////////// 임시 코드데이터
    protected const float m_kUnitLength = 0.5f;
    protected const float m_kWallHeight = 1.5f;

    public const float kIsometricVerticalAngle = 30.0f;
}
