using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingScene : MonoBehaviour {

    public Material paintingMat;
    public float micThreshold;
    public Material wipeMat;
    bool usingWipe = true;
    OurTownGestureListener gesture;
    bool next = false;

	// Use this for initialization
	void Start () {
        gesture = OurTownGestureListener.Instance;
        Reset();
    }

    void Reset()
    {
        usingWipe = true;
        next = false;
        wipeMat.SetFloat("_Value", 1.05f);
        StartCoroutine("RunScene");
    }
	
	// Update is called once per frame
	void Update () {
        //print(MicListener.loudness);
        paintingMat.SetFloat("_Loudness", MicListener.loudness);
        paintingMat.SetFloat("_Edge", micThreshold);
        if (Input.GetButtonDown("Jump"))
        {
            next = true;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (usingWipe)
        {
            Graphics.Blit(source, dest, wipeMat);
        }
        else
        {
            Graphics.Blit(paintingMat.mainTexture, dest, paintingMat);
        }
    }

    IEnumerator RunScene()
    {
        usingWipe = true;
        wipeMat.SetFloat("_Value", 1.05f);
        gesture.SetCurrentGesture(KinectGestures.Gestures.TheMoreYouKnow); //wipe
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            //Blit();
            yield return null;
        }
        next = false;
        float startTime = Time.time;
        float fadeDuration = 3f;
        while (Time.time - startTime < fadeDuration + .1f)
        {
            wipeMat.SetFloat("_Value", 1 - (Time.time - startTime) / fadeDuration);
            yield return null;
        }
        wipeMat.SetFloat("_Value", -0.05f);
        usingWipe = false;
    }

    void OnDisable()
    {
        Reset();
    }
}
