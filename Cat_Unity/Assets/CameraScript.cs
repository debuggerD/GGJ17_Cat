using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    GameObject cat;
	// Use this for initialization
	void Start () {
        cat = GameObject.Find("Cat");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = cat.transform.position + new Vector3(m_kCameraDistanceFactor, m_kCameraDistanceFactor * m_kCameraHeightFactor, -m_kCameraDistanceFactor);
	}


    //////////////////////////////////////////////////////////////////////////////// 임시 코드데이터
    protected const float m_kCameraDistanceFactor = 5.0f;
    protected const float m_kCameraHeightFactor = 0.8f;
}
