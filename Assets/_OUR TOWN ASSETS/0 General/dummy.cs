using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummy : MonoBehaviour {

    public Material mat;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnRenderImage(RenderTexture s, RenderTexture d)
    {
        Graphics.Blit(s, d, mat);
    }
}
