using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour {

    GameObject Cat;
    MeshRenderer meshRenderer;
    public Material wallMaterial;
    public Material transparentMaterial;

	// Use this for initialization
	void Start () {
        Cat = GameObject.Find("Cat");
        meshRenderer = GetComponent<MeshRenderer>();
	}

    bool transparent = false;

	// Update is called once per frame
	void Update () {
        if (!transparent)
        {
            if (Cat.transform.position.x > transform.position.x && Cat.transform.position.z > transform.position.z)
            {
                transparent = true;
                meshRenderer.material = transparentMaterial;
            }
        }
        else
        {
            if (Cat.transform.position.x < transform.position.x || Cat.transform.position.z < transform.position.z)
            {
                transparent = false;
                meshRenderer.material = wallMaterial;
            }
        }
	}
}
