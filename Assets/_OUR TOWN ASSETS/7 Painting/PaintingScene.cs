using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingScene : MonoBehaviour {

    public Material paintingMat;
    public float micThreshold;

	// Use this for initialization
	void Start () {
		
	}

    void Reset()
    {

    }
	
	// Update is called once per frame
	void Update () {
        //print(MicListener.loudness);
        paintingMat.SetFloat("_Loudness", MicListener.loudness);
        paintingMat.SetFloat("_Edge", micThreshold);
	}

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(paintingMat.mainTexture, dest, paintingMat);
    }

    void OnDisable()
    {
        Reset();
    }
}
