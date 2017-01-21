using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManagerScript : MonoBehaviour {

    public GameObject Tile;

    public GameObject NewWall;
    public GameObject WallCollider;

    public GameObject BlockerCollider;

    int width = 0;
    int length = 0;

    int[,] map_move;
    int[,] map_height;
    public int[,] map_wave_pass;
    public int[,] map_wave_modify;
    int[,] map_wall;

    protected GameObject[,] m_wallObjects;
    protected GameObject[,] m_moveBlockerObjects;

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
        string[] textArray = txtFile.text.Trim().Split('\t');
        width = Int32.Parse(textArray[0]);
        length = Int32.Parse(textArray[1]);
    }

    int[,] FileToArray(string filename, int w, int h)
    {
        int[,] result = new int[w, h];
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
        map_move = FileToArray("map_move_m", width, length);
        map_height = FileToArray("map_height_m", width, length);
        map_wave_pass = FileToArray("map_wave_pass_m", width, length);
        map_wave_modify = FileToArray("map_wave_modify_m", width, length);
        map_wall = FileToArray("map_wall_m", width, length);
        CreateMap();
    }

    void CreateMap()
    {
        _CreatTileObjects();
        _CreateWallObjects();
        _CreateMoveBlockers();
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

    public void NotifyCatMove(float gridI, float gridJ, bool isNormalMove)
    {
        int movableRegion = GetMovableRegion(gridI, gridJ);
        if (movableRegion != m_currentMoveRegion)
        {
            _NotifyMoveRegionChange(movableRegion);
        }
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

    protected void ApplyNormalWallTransform(GameObject wallObj, int i, int j)
    {
        wallObj.transform.position = new Vector3(GetXPos(i, j + 1), 0, GetZPos(i, j + 1));
    }

    #endregion


    #region Movable

    protected int m_currentMoveRegion;

    protected void _CreateMoveBlockers()
    {
        m_currentMoveRegion = -1;

        m_moveBlockerObjects = new GameObject[width, length];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (map_move[i, j] == 0)
                {
                    if (map_wall[i, j] == 0 || map_wall[i, j] == 2)
                    {
                        continue;
                    }
                }

                GameObject createdCollider = Instantiate(BlockerCollider);
                createdCollider.transform.position = new Vector3(GetXPos(i, j), 0, GetZPos(i, j));
                createdCollider.transform.localScale = new Vector3(m_kUnitLength, m_kBlockerHeight, m_kUnitLength);
                m_moveBlockerObjects[i, j] = createdCollider;
                createdCollider.SetActive(map_move[i, j] == 0);
            }
        }
    }

    public int GetMovableRegion(float gridI, float gridJ)
    {
        if (gridI < 0.0f || gridI >= width || gridJ < 0.0f || gridJ >= length)
        {
            return - 1;
        }
        return map_move[(int)Math.Floor(gridI), (int)Math.Floor(gridJ)];
    }

    protected void _NotifyMoveRegionChange(int newRegion)
    {
        if (newRegion == m_currentMoveRegion)
        {
            return;
        }

        if (m_currentMoveRegion > 0)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    GameObject currentBlocker = m_moveBlockerObjects[i, j];
                    if (currentBlocker == null)
                    {
                        continue;
                    }

                    if (map_move[i, j] >= 1)
                    {
                        currentBlocker.SetActive(false);
                    }
                }
            }
        }

        m_currentMoveRegion = newRegion;
        if (newRegion > 0)
        {
            if (newRegion == 1)
            {
                // One Tile
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        if (map_move[i, j] > 1)
                        {
                            GameObject currentBlocker = m_moveBlockerObjects[i, j];
                            if (currentBlocker == null)
                            {
                                continue;
                            }
                            currentBlocker.SetActive(true);
                        }
                    }
                }
            }
            else if (newRegion >= 128)
            {
                if (newRegion % 2 == 0)
                {
                    // Only To
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < length; j++)
                        {
                            if (map_move[i, j] != newRegion && map_move[i, j] > 0)
                            {
                                GameObject currentBlocker = m_moveBlockerObjects[i, j];
                                if (currentBlocker == null)
                                {
                                    continue;
                                }
                                currentBlocker.SetActive(true);
                            }
                        }
                    }
                }
                else
                {
                    bool isBlockOne = (newRegion % 4 == 1);

                    // Move Down
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < length; j++)
                        {
                            GameObject currentBlocker = m_moveBlockerObjects[i, j];
                            if (currentBlocker == null)
                            {
                                continue;
                            }
                            if (map_move[i, j] == 0)
                            {
                                continue;
                            }

                            if (map_move[i, j] == 1)
                            {
                                currentBlocker.SetActive(isBlockOne);
                            }
                            else if (map_move[i, j] > newRegion)
                            {
                                currentBlocker.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////// 임시 코드데이터
    protected const float m_kUnitLength = 0.5f;
    protected const float m_kWallHeight = 1.5f;

    protected const float m_kBlockerHeight = 10.0f;

    public const float kIsometricVerticalAngle = 30.0f;
}
