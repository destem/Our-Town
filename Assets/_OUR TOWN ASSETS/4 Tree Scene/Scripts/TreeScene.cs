﻿#define BLIT_TO_SCREEN

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeScene : MonoBehaviour {

    public Texture2D startMask;
    public Texture2D MaskOneTex;
    public Texture2D MaskTwoTex;
    public Texture2D MaskThreeTex;
    public Texture2D MaskFourTex;
    public Texture2D black;
    public Texture2D full;
    public Texture2D paper;
    public Texture2D chapelOnly;
    public Material chapelMat;
    public Material growMat;
    public Material displayMat;
    //public Material packer;
    Material growMat2;
    RenderTexture buff;
    RenderTexture buff2;
    RenderTexture final;
    RenderTexture final2;
    public Text t;
    float brushSize = 10f;
    OurTownGestureListener gesture;
    //public GameObject screenModel;
    public float slowSpeed = 0f;
    public float mediumSpeed = 0f;
    public float fastSpeed = 0f;
    public int iterations = 1;
    public float growthThreshhold = 1f;
    public float chapelFadeTime = 2f;
    bool next = false;
    bool usingGrowth = false;
    
    RenderTexture _createTexture(int w, int h)
    {
        RenderTexture t = new RenderTexture(w, h, 0, RenderTextureFormat.ARGBFloat);
        t.filterMode = FilterMode.Point;
        t.anisoLevel = 1;
        t.autoGenerateMips = false;
        t.useMipMap = false;
        t.Create();
        return t;
    }

    // Use this for initialization
    void Start()
    {
        //screenModel.GetComponent<Renderer>().material = chapelMat;
        //buff = _createTexture(startMask.width, startMask.height);
        //buff2 = _createTexture(startMask.width, startMask.height); 
        //final = _createTexture(startMask.width, startMask.height);
        //final2 = _createTexture(startMask.width, startMask.height);

        //displayMat.SetTexture("_MainTex", buff);
        //displayMat.SetTexture("_SecondTex", buff2);
        //growMat.SetTexture("_MaskOneTex", black);
        //growMat.SetTexture("_MaskTwoTex", black);
        //growMat2.SetTexture("_MaskOneTex", black);
        //growMat2.SetTexture("_MaskTwoTex", black);

        gesture = OurTownGestureListener.Instance;
        //chapelMat.SetVector("_Value", new Vector4(0f, 0f, 0f, 0f));
        //ResetUVs();
        //StartCoroutine(RunTreeScene());
        Reset();
    }

    void Reset()
    {
        StopAllCoroutines();
        usingGrowth = false;
        //screenModel.GetComponent<Renderer>().material = chapelMat;
        chapelMat.SetVector("_Value", new Vector4(1f, 0f, 0f, 0f));
        chapelMat.SetTexture("_Paper", paper);
        chapelMat.SetTexture("_Chapel", chapelOnly);

        buff = _createTexture(startMask.width, startMask.height);
        buff2 = _createTexture(startMask.width, startMask.height);
        final = _createTexture(startMask.width, startMask.height);
        final2 = _createTexture(startMask.width, startMask.height);
        growMat2 = new Material(growMat);
        growMat.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));
        growMat2.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));
        //Graphics.Blit(startMask, buff, packer);
        displayMat.SetTexture("_MainTex", buff);
        displayMat.SetTexture("_SecondTex", buff2);
        growMat.SetTexture("_MaskOneTex", black);
        growMat.SetTexture("_MaskTwoTex", black);
        growMat2.SetTexture("_MaskOneTex", black);
        growMat2.SetTexture("_MaskTwoTex", black);
        ResetUVs();

        StartCoroutine(RunTreeScene());
    }

    // Update is called once per frame
    void Update()
    {
      
        ResetUVs();
        next = false;
           
        //Vector3 mousePos = Input.mousePosition;
        //t.text = string.Format("{0:0.000}, {1:0.000}", mousePos.x / Screen.width, mousePos.y / Screen.height);

        if (Input.GetButtonDown("Jump"))
        {
            next = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }

    }

#if BLIT_TO_SCREEN
    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (usingGrowth)
        {
            for (int i = 0; i < iterations; i++)
            {
                Graphics.Blit(buff, final, growMat);
                Graphics.Blit(final, buff, growMat);
                Graphics.Blit(buff2, final2, growMat2);
                Graphics.Blit(final2, buff2, growMat2);

            }
            Graphics.Blit(buff, dest, displayMat);
            //Graphics.Blit(buff, dest);
        }
        else
        {
            Graphics.Blit(buff, dest, chapelMat);
        }

    }

#endif

    IEnumerator RunTreeScene()
    {
        print("Starting tree scene");
        yield return new WaitForSeconds(1f); //gesture not getting initialized fast enough??
        print("Waiting to activate lean forward gesture to start first words");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for lean forward");
        gesture.SetCurrentGesture(KinectGestures.Gestures.LeanForward);
        gesture.ClearGestureSuccess();
        while (!next  && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("First words");
        usingGrowth = true;
        //screenModel.GetComponent<Renderer>().material = displayMat;
        growMat.SetTexture("_MaskOneTex", MaskOneTex);
        growMat.SetTexture("_MaskTwoTex", MaskTwoTex);
        growMat2.SetTexture("_MaskOneTex", MaskThreeTex);
        growMat2.SetTexture("_MaskTwoTex", MaskFourTex);
        SetMaskThree(.215f, .994f);
        yield return null;
        SetMaskThree(.222f, .942f);
        yield return null;
        SetMaskThree(.24f, .886f);
        yield return null;
        SetMaskThree(.58f, .065f);
        yield return null;
        SetMaskThree(.335f, .15f);
        yield return null;
        print("Waiting to activate psi gesture to start first branches");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for psi");
        gesture.SetCurrentGesture(KinectGestures.Gestures.Psi);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("First branches");
        SetMaskOne(0.500f, 0.879f);
        yield return null;
        print("Waiting to activate shrug gesture to start central growth");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for shrug");
        gesture.SetCurrentGesture(KinectGestures.Gestures.Shrug);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("Central growth");
        SetMaskTwo(.425f, .01f);
        yield return null;
        SetMaskTwo(.446f, .01f);
        yield return null;
        SetMaskTwo(.553f, .01f);
        yield return null;
        SetMaskTwo(.574f, .01f);
        print("Waiting to activate lean right gesture to start growth from sides");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for lean right");
        gesture.SetCurrentGesture(KinectGestures.Gestures.LeanRight);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("Growth from sides");
       
        SetMaskTwo(.004f, .01f);
        yield return null;
        SetMaskTwo(.02f, .01f);
        yield return null;
        SetMaskOne(.055f, .01f);
        yield return null;
        SetMaskOne(.078f, .01f);
        yield return null;
        SetMaskOne(.09f, .01f);
        yield return null;
        SetMaskOne(.103f, .01f);
        yield return null;
        SetMaskOne(.11f, .01f);
        yield return null;
        SetMaskThree(.391f, .005f);
        yield return null;
        SetMaskOne(.965f, .01f);
        yield return null;
        SetMaskOne(.98f, .01f);
        yield return null;
        SetMaskOne(.999f, .01f);
        yield return null;
        SetMaskOne(.595f, .01f);
        yield return null; print("Waiting to activate point forward gesture to start second words");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for point forward");
        gesture.SetCurrentGesture(KinectGestures.Gestures.PointForward);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("Second set of words");
        SetMaskTwo(.196f, .019f);
        yield return null;
        SetMaskTwo(.209f, .029f);
        yield return null;
        SetMaskTwo(.231f, .032f);
        yield return null;
        SetMaskTwo(.248f, .023f);
        yield return null;
        SetMaskTwo(.023f, .97f);
        yield return null;
        SetMaskOne(.254f, .726f);
        yield return null;
        SetMaskOne(.74f, .82f);
        yield return null;
        SetMaskOne(.594f, .462f);
        yield return null;
        print("Waiting to activate clench gesture to start stage 5 growth");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for clench");
        gesture.SetCurrentGesture(KinectGestures.Gestures.Clench);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("Stage 5 fast, starting timer for next phase");
        SetMaskOne(.275f, .005f);
        yield return null;
        SetMaskOne(.36f, .005f);
        yield return null;
        SetMaskOne(.375f, .005f);
        yield return null;
        SetMaskTwo(.326f, .005f);
        yield return null;
        SetMaskThree(.834f, .01f);
        yield return null;
        SetMaskThree(.767f, .01f);
        yield return null;
        SetMaskThree(.778f, .01f);
        yield return null;
        SetMaskThree(.784f, .01f);
        yield return null;
        SetMaskThree(.788f, .01f);
        yield return null;
        SetMaskThree(.81f, .01f);
        yield return null;
        SetMaskThree(.845f, .01f);
        yield return null;
        SetMaskThree(.858f, .01f);
        yield return null;
        SetMaskThree(.877f, .01f);
        yield return null;
        SetMaskThree(.899f, .01f);
        yield return null;
        SetMaskThree(.912f, .01f);
        yield return new WaitForSeconds(10f);
        print("Stage 5 fast - stage 2");
        SetMaskTwo(.687f, .005f);
        yield return null;
        SetMaskTwo(.697f, .005f);
        yield return null;
        SetMaskTwo(.183f, .005f);
        yield return null;
        SetMaskTwo(.202f, .005f);
        yield return null;
        SetMaskTwo(.224f, .005f);
        yield return null;
        SetMaskOne(.126f, .005f);
        yield return null;
        SetMaskOne(.156f, .005f);
        yield return null;
        SetMaskOne(.727f, .005f);
        yield return null;
        SetMaskOne(.748f, .005f);
        yield return null;
        print("Waiting to activate shrug gesture to start third set of words");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for shrug");
        gesture.SetCurrentGesture(KinectGestures.Gestures.Shrug);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("Third set of words");
        SetMaskOne(.401f, .027f);
        yield return null;
        SetMaskOne(.442f, .84f);
        yield return null;
        SetMaskOne(.594f, .371f);
        yield return null;
        print("Waiting to activate clap gesture to start final words");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for clap");
        gesture.SetCurrentGesture(KinectGestures.Gestures.Clap);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("Final set of words");
        SetMaskTwo(.027f, .815f);
        yield return null;
        SetMaskOne(.744f, .502f);
        yield return null;
        SetMaskTwo(.652f, .686f);
        yield return null;
        SetMaskOne(.933f, .563f);
        yield return null;
        SetMaskOne(.956f, .97f);
        yield return null;
        print("Waiting to activate The More You Know gesture to start final fade");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for The More You Know");
        gesture.SetCurrentGesture(KinectGestures.Gestures.TheMoreYouKnow);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        chapelMat.SetVector("_Value", new Vector4(0f, 0f, 0f, 0f));

        print("Fade to black");
        usingGrowth = false;
        chapelMat.SetTexture("_Paper", full);
        chapelMat.SetTexture("_Chapel", black);

        float startTime = Time.time;
        while (Time.time - startTime < 2f)
        {
            chapelMat.SetVector("_Value", new Vector4((Time.time - startTime) / 2f, 0f, 0f, 0f));
            yield return null;
        }
        chapelMat.SetVector("_Value", new Vector4(1f, 0f, 0f, 0f));
        print("End of Act II");
    }

    public void SetMaskOne(float u, float v)
    {
        growMat.SetVector("_MaskOneCoords", new Vector4(u, v, brushSize, -1f));
    }

    public void SetMaskTwo(float u, float v)
    {
        growMat.SetVector("_MaskTwoCoords", new Vector4(u, v, brushSize, -1f));
    }

    public void SetMaskThree(float u, float v)
    {
        growMat2.SetVector("_MaskOneCoords", new Vector4(u, v, brushSize, -1f));
    }

    public void SetMaskFour(float u, float v)
    {
        growMat2.SetVector("_MaskTwoCoords", new Vector4(u, v, brushSize, -1f));
    }

    public void ResetUVs()
    {
        growMat.SetVector("_MaskOneCoords", new Vector4(-1, -1, -1, -1));
        growMat.SetVector("_MaskTwoCoords", new Vector4(-1, -1, -1, -1));
        growMat2.SetVector("_MaskOneCoords", new Vector4(-1, -1, -1, -1));
        growMat2.SetVector("_MaskTwoCoords", new Vector4(-1, -1, -1, -1));
    }

    IEnumerator EnableScreenCollision()
    {
        yield return new WaitForSeconds(5f);
        //screenModel.GetComponent<MeshCollider>().enabled = true;
    }

    void Blit()
    {
#if !BLIT_TO_SCREEN
        if (usingGrowth){
            for (int i = 0; i < iterations; i++)
                {
                    Graphics.Blit(buff, final, growMat);
                    Graphics.Blit(final, buff, growMat);
                }
        }
#endif
        return;
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
