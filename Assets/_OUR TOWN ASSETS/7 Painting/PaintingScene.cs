using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingScene : MonoBehaviour {

    public Material paintingMat;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //print(MicListener.loudness);
        paintingMat.SetFloat("_Loudness", MicListener.loudness);
	}

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(paintingMat.mainTexture, dest, paintingMat);
    }
}
