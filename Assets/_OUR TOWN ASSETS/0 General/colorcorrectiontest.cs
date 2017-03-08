using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorcorrectiontest : MonoBehaviour {

    public Texture2D tex;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(tex, dest);
    }
}
