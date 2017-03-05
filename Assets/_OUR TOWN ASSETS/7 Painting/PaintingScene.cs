using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingScene : MonoBehaviour {

    public Material micMat;
    public float micThreshold;
    public Material wipeMat;
    public Texture2D finalPainting;
    bool usingWipe = true;
    OurTownGestureListener gesture;
    bool next = false;
    enum PaintingRenderType {WipeIn, Building, Mic, Breakdown }
    PaintingRenderType paintRender = PaintingRenderType.WipeIn;

	// Use this for initialization
	void Start () {
        gesture = OurTownGestureListener.Instance;
        Reset();
    }

    void Reset()
    {
        usingWipe = true;
        next = false;
        paintRender = PaintingRenderType.WipeIn;
        wipeMat.SetFloat("_Value", 1.05f);
        micMat.SetFloat("_Loudness", 0);
        micMat.SetFloat("_Edge", micThreshold);
        StartCoroutine("RunScene");
    }
	
	// Update is called once per frame
	void Update () {
        //print(MicListener.loudness);
        if (paintRender == PaintingRenderType.Mic)
        {
            micMat.SetFloat("_Loudness", MicListener.loudness);
            micMat.SetFloat("_Edge", micThreshold);
        }
        if (Input.GetButtonDown("Jump"))
        {
            next = true;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        switch (paintRender)
        {
            case PaintingRenderType.WipeIn:
                Graphics.Blit(source, dest, wipeMat);
                break;
            case PaintingRenderType.Mic:
                Graphics.Blit(finalPainting, dest, micMat);
                break;

        }
       
    }

    IEnumerator RunScene()
    {
       // usingWipe = true;
        wipeMat.SetFloat("_Value", -0.05f);
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
            wipeMat.SetFloat("_Value",(Time.time - startTime) / fadeDuration);
            yield return null;
        }
        wipeMat.SetFloat("_Value", 1.05f);
        //usingWipe = false;
        paintRender = PaintingRenderType.Mic;
        gesture.SetCurrentGesture(KinectGestures.Gestures.ForearmWave); //wipe
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            //Blit();
            yield return null;
        }
        next = false;
        OurTownManager.GotoStar();
    }

    void OnDisable()
    {
        Reset();
    }
}
