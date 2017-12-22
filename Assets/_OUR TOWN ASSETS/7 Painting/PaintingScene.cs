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
    
    public Material buildMat;

	// Use this for initialization
	void Start () {
        gesture = OurTownGestureListener.Instance;
        buildMat.SetTexture("_Painting", OurTownManager.paintingArray);
        micMat.SetTexture("_Painting", OurTownManager.paintingArray);
        Reset();
    }

    void Reset()
    {
        usingWipe = true;
        next = false;
        GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>().enabled = true;
        paintRender = PaintingRenderType.WipeIn;
        wipeMat.SetFloat("_Value", 1.05f);
        micMat.SetFloat("_Loudness", 0);
        micMat.SetFloat("_Edge", micThreshold);
        StartCoroutine("RunPaintingScene");
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
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
            case PaintingRenderType.Building:
                Graphics.Blit(source, dest, buildMat);
                break;
        }
       
    }

    IEnumerator RunPaintingScene()
    {
       // usingWipe = true;
        wipeMat.SetFloat("_Value", -0.05f);
        print("Waiting to activate Behold gesture to start painting");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for Behold");
        gesture.SetCurrentGesture(KinectGestures.Gestures.Behold); //wipe
        gesture.ClearGestureSuccess();
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            //Blit();
            yield return null;
        }
        next = false;
        print("start painting");
        float startTime = Time.time;
        float fadeDuration = 5f;
        while (Time.time - startTime < fadeDuration + .1f)
        {
            wipeMat.SetFloat("_Value",(Time.time - startTime) / fadeDuration);
            yield return null;
        }
        wipeMat.SetFloat("_Value", 1.05f);
        //usingWipe = false;
        paintRender = PaintingRenderType.Building;

        //TOTAL OF FOUR MINUTES
        for (int i = 0; i < 17; i++)
        {
            startTime = Time.time;
            fadeDuration = 16f; //1 for mic test, 16 for show
            while (Time.time - startTime < fadeDuration)
            {
                buildMat.SetVector("level", new Vector4(i, (Time.time - startTime) / fadeDuration, 0f, 0f));
                yield return null;
            }
        }
        print("Waiting to activate mic on 'Take me back'");
        while (!next)
        {
            yield return null;
        }
        next = false;
        paintRender = PaintingRenderType.Mic; //use mic inputs
        print("Accepting mic levels");

        for (int i = 17; i > 1; i--)
        {
            startTime = Time.time;
            fadeDuration = 2f;
            while (Time.time - startTime < fadeDuration)
            {
                micMat.SetVector("level", new Vector4(i, (Time.time - startTime) / fadeDuration, 0f, 0f));
                yield return null;
            }
        }

        print("Waiting to kill painting on 'No'");
        //gesture.SetCurrentGesture(KinectGestures.Gestures.ForearmWave); //wipe
        while (!next)
        {         
            yield return null;
        }
        next = false;
        OurTownManager.GotoStar();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnEnable()
    {
        Reset();
    }

    //public void Stop()
    //{
    //    StopAllCoroutines();
    //}
}
