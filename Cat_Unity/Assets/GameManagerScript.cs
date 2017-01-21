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

    public GameObject Object_Cage;
    public GameObject Object_Door;
    public GameObject Object_Laser;
    public GameObject Object_CatFood;

    int width = 0;
    int length = 0;

    public int[,] map_move;
    int[,] map_height;
    public int[,] map_wave_pass;
    public int[,] map_wave_modify;
    int[,] map_wall;

	public int[,] seeing;

    protected GameObject[,] m_wallObjects;
    protected GameObject[,] m_moveBlockerObjects;

    // Use this for initialization
    void Start () {
        GetMetaData();
        ReadMapData();

        _InitializeGameObjectData();

		LoadAchievements ();

		warp_count = 0;
    }
	public int warp_count = 0;
	int[] achievements = new int[]{0,0,0,0,0};
	float achievement_start_time = - 100;
	int achievement_index;
	public enum Achivements {
		LuckyCat,
		CatHavingABadDay,
		HeisenburgUncatiantyPrinciple,
		TimeTravel,
		RoadToFreedom,
	};

	public void GetAchivement(Achivements a)
	{
		if (achievements [(int)a] != 0)
			return;
		achievement_start_time = Time.time;
		achievement_index = (int)a;
		achievements[(int)a] = 1;
	}

	void OnGUI()
	{
		var msgs = new string[] {
			"Lucky Cat\nYou exit the first room with just one try!",
			"Cat Having a Bad Day\nYou are unlucky...",
			"Heisenburg Un`cat'ianty Principle\nYou are observed therefore you exist.",
			"Time Travel\nFrom the beginning, again.",
			"Road to Freedom\nYou finished the game.",
		};
		var dt = Time.time - achievement_start_time;
		if (dt < 5f) {
			print (dt + " " + Time.time + " " + achievement_start_time);
			if (dt < 1f) {
				GUI.Box (new Rect (Screen.width / 10, 0, Screen.width * 8 / 10, Screen.height / 10*dt), msgs[achievement_index]);
			} else if (dt < 4f) {
				dt = (dt - 1f) / 3f;
				GUI.Box (new Rect (Screen.width / 10, 0, Screen.width * 8 / 10, Screen.height / 10), msgs[achievement_index]);
			} else {
				dt = (5f-dt);
				GUI.Box (new Rect (Screen.width / 10, 0, Screen.width * 8 / 10, Screen.height / 10*dt), msgs[achievement_index]);
			}
		}
	}

	void SaveAchievements()
	{
		return;
		var fs = System.IO.File.Create (Application.persistentDataPath + "/achieve.dat");
		var tw = new System.IO.StreamWriter (fs);
		tw.WriteLine(string.Join (" ", achievements.Select (p => p + "").ToArray()));
	}

	void LoadAchievements()
	{
		if (System.IO.File.Exists (Application.persistentDataPath + "/achieve.dat")) {
			var fs = System.IO.File.Open (Application.persistentDataPath + "/achieve.dat", System.IO.FileMode.Open);
			var tr = new System.IO.StreamReader (fs);
			var line = tr.ReadLine ();
			var tokens = line.Trim ().Split (' ');
			var nums = tokens.Select (p => int.Parse (p));

		}
	}
	
	// Update is called once per frame
	void Update () {
		for (var i = 0; i < seeing.GetLength (0); i++)
			for (var j = 0; j < seeing.GetLength (1); j++) {
				seeing [i, j] = 0;
			}
	}

    void GetMetaData()
    {
        TextAsset txtFile = (TextAsset)Resources.Load("map_meta") as TextAsset;
        string[] textArray = txtFile.text.Trim().Split('\t');
        width = Int32.Parse(textArray[0]);
        length = Int32.Parse(textArray[1]);

		seeing = new int[width*2, length*2];
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

        _InitializeWallHidingData();

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

        int wallHideRegion = GetWallHideRegion(gridI, gridJ);
        if (wallHideRegion != m_currentWallHideRegion)
        {
            _NotifyWallHideRegionChange(wallHideRegion);
        }
		if (warp_count >= 7 && wallHideRegion < 2 && achievements[(int)Achivements.CatHavingABadDay] == 0) {
			achievement_start_time = Time.time;
			achievement_index = (int)Achivements.CatHavingABadDay;
			achievements[(int)Achivements.CatHavingABadDay] = 1;
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


    #region Wall Hiding

    int[,] m_mapWallHideRegion;
    Dictionary<int, bool[,]> m_mapWallHidingTable;

    protected int m_currentWallHideRegion;

    protected void _InitializeWallHidingData()
    {
        m_currentWallHideRegion = -1;

        // Region
        m_mapWallHideRegion = new int[width, length];
        TextAsset txtFile = (TextAsset)Resources.Load("map_wall_hide") as TextAsset;
        string[] lines = txtFile.text.Trim().Split('\n');
        for (int i = 0; i < length; i++)
        {
            string[] line = lines[i].Split('\t');
            for (int j = 0; j < width; j++)
            {
                m_mapWallHideRegion[j, i] = Int32.Parse(line[j]);
            }
        }

        // Hide Map
        m_mapWallHidingTable = new Dictionary<int, bool[,]>();
        int lineCount = lines.Length;
        int readLine = length;
        while (readLine + 1 < lineCount)
        {
            int currentRegion = Int32.Parse(lines[readLine]);
            ++readLine;

            bool[,] currentTable = new bool[width, length];
            for (int i = 0; i < length; i++)
            {
                string[] line = lines[readLine + i].Split('\t');
                for (int j = 0; j < width; j++)
                {
                    currentTable[j, i] = (Int32.Parse(line[j]) == 1);
                }
            }

            m_mapWallHidingTable.Add(currentRegion, currentTable);
            readLine += length;
        }
    }

    public int GetWallHideRegion(float gridI, float gridJ)
    {
        if (gridI < 0.0f || gridI >= width || gridJ < 0.0f || gridJ >= length)
        {
            return -1;
        }
        return m_mapWallHideRegion[(int)Math.Floor(gridI), (int)Math.Floor(gridJ)];
    }

    protected void _NotifyWallHideRegionChange(int newRegion)
    {
        if (newRegion == m_currentWallHideRegion)
        {
            return;
        }

        if (m_mapWallHidingTable == null)
        {
            return;
        }

		if (newRegion==2 && warp_count == 2 && achievements[(int)Achivements.LuckyCat] == 0)
		{
			achievement_start_time = Time.time;
			achievement_index = (int)Achivements.LuckyCat;
			achievements[(int)Achivements.LuckyCat] = 1;
		}

        bool[,] isHideTable = null;
        if (m_mapWallHidingTable.ContainsKey(newRegion))
        {
            isHideTable = m_mapWallHidingTable[newRegion];
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                GameObject currentWall = m_wallObjects[i, j];
                if (currentWall == null)
                {
                    continue;
                }

                WallUpdater updaterObject = currentWall.GetComponent<WallUpdater>();
                if (isHideTable == null)
                {
                    updaterObject.SetIsNeedToUseFullSize(true);
                }
                else
                {
                    updaterObject.SetIsNeedToUseFullSize(!isHideTable[i, j]);
                }
            }
        }

        m_currentWallHideRegion = newRegion;
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


    #region Game Objects

    protected Dictionary<int, GameObject> m_objectTable;

    protected void _InitializeGameObjectData()
    {
        // Hardcoded Object Classes
        m_objectTable = new Dictionary<int, GameObject>();
        m_objectTable.Add(1, Object_Cage);
        m_objectTable.Add(2, Object_Door);
        m_objectTable.Add(3, Object_Laser);
        m_objectTable.Add(4, Object_CatFood);

        // Objects
        TextAsset txtFile = (TextAsset)Resources.Load("map_objects") as TextAsset;
        string[] lines = txtFile.text.Trim().Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split('\t');
            if (line.Length == 0)
            {
                continue;
            }
            if (line[0].Trim().StartsWith("#"))
            {
                continue;
            }

            // Read Object Data
            //int objectId = int.Parse(line[0]);
            int gridI = int.Parse(line[1]);
            int gridJ = int.Parse(line[2]);
            int objectWidth = int.Parse(line[3]);
            int objectHeight = int.Parse(line[4]);
            //int logicRef = int.Parse(line[5])
            int objectClassId = int.Parse(line[6]);
            bool isFlip = (int.Parse(line[7]) == 1);

            // Create Object
            GameObject createdGameObject = null;
            if (m_objectTable.ContainsKey(objectClassId))
            {
                createdGameObject = Instantiate(m_objectTable[objectClassId]);
            }
            if (createdGameObject == null)
            {
                continue;
            }

            createdGameObject.transform.position = new Vector3(GetXPos(gridI, gridJ + objectHeight), 0, GetZPos(gridI, gridJ + objectHeight));
            if (isFlip)
            {
                createdGameObject.transform.localScale = new Vector3(-createdGameObject.transform.localScale.x, createdGameObject.transform.localScale.y, createdGameObject.transform.localScale.z);
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
