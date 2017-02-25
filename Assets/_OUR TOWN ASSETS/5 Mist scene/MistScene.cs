using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistScene : MonoBehaviour {

    public GameObject screen;
    public Material scrollMat;
    public float scrollSpeed;
    float offset = 0f;

	// Use this for initialization
	void Start () {
        screen.GetComponent<Renderer>().material = scrollMat;
	}
	
	// Update is called once per frame
	void Update () {
        offset += Time.deltaTime * scrollSpeed;
        scrollMat.mainTextureOffset = new Vector2(offset, 0f);
        scrollMat.SetFloat("_offset", offset);
	}

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(scrollMat.mainTexture, dest, scrollMat);
    }
}
