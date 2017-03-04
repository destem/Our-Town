using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistScene : MonoBehaviour {

    //public GameObject screen;
    public Material scrollMat;
    public float scrollSpeed;
    float offset = 0f;
    OurTownGestureListener gesture;
    bool next = false;

    // Use this for initialization
    void Start () {
        //screen.GetComponent<Renderer>().material = scrollMat;
        gesture = OurTownGestureListener.Instance;
        Reset();
    }

    void Reset()
    {
        StopAllCoroutines();
        StartCoroutine("RunScene");
    }
	
	// Update is called once per frame
	void Update () {
        //offset += Time.deltaTime * scrollSpeed;
        //scrollMat.mainTextureOffset = new Vector2(offset, 0f);
        //scrollMat.SetFloat("_offset", offset);
        next = false;

        if (Input.GetButtonDown("Jump"))
        {
            next = true;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(scrollMat.mainTexture, dest, scrollMat);
    }

    IEnumerator RunScene()
    {
        yield return new WaitForSeconds(1f); //gesture not getting initialized fast enough??
        gesture.SetCurrentGesture(KinectGestures.Gestures.TheMoreYouKnow);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        OurTownManager.GotoOcean();
    }
}
