using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallUpdater : MonoBehaviour {

    public Sprite FullSizeWall;
    public Sprite ClippedSizeWall;

    public GameObject CurrentCat;

    public int gridI;
    public int gridJ;

    protected SpriteRenderer m_mySpriteRenderer;
    protected bool m_isUsingFullSizeWall;

    protected bool m_isNeedToUseFullSize = true;

    // Use this for initialization
    void Start () {
        m_mySpriteRenderer = GetComponent<Renderer>() as SpriteRenderer;
        m_isUsingFullSizeWall = true;
        CurrentCat = GameObject.FindWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
		if (m_mySpriteRenderer == null)
        {
            return;
        }

        bool isNeedToUsingFullSizeWall = _CheckIsNeedToUseFullSize();
        if (isNeedToUsingFullSizeWall == m_isUsingFullSizeWall)
        {
            return;
        }

        m_mySpriteRenderer.sprite = isNeedToUsingFullSizeWall ? FullSizeWall : ClippedSizeWall;
        m_isUsingFullSizeWall = isNeedToUsingFullSizeWall;

    }

    protected bool _CheckIsNeedToUseFullSize()
    {
        return m_isNeedToUseFullSize;
    }

    public void SetIsNeedToUseFullSize(bool isNeedToUseFullSize)
    {
        m_isNeedToUseFullSize = isNeedToUseFullSize;
    }
}
