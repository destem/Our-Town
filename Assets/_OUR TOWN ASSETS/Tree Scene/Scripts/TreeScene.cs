﻿#define BLIT_TO_SCREEN

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeScene : MonoBehaviour {

    public Texture2D startMask;
    public Texture2D chapelMask;
    //public Texture2D nextMask;
    public Texture2D MaskOneTex;
    public Texture2D MaskTwoTex;
    public Texture2D MaskThreeTex;
    public Texture2D MaskFourTex;
    public Texture2D full;
    public Material chapelMat;
    public Material growMat;
    public Material displayMat;
    public Material packer;
    RenderTexture buff;
    RenderTexture final;
    public Text t;
    float brushSize = 10f;
    //bool addingToMask = false;
    TreeGestureListener gesture;
    public GameObject screenModel;
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
        growMat.SetTexture("_MaskOneTex", chapelMask);
        growMat.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));
        buff = _createTexture(startMask.width, startMask.height);
        final = _createTexture(startMask.width, startMask.height);
        Graphics.Blit(startMask, buff, packer);
        displayMat.SetTexture("_MainTex", buff);
        gesture = TreeGestureListener.Instance;
        chapelMat.SetVector("_Value", new Vector4(0f, 0f, 0f, 0f));
        //chapelTex = startMask;
        StartCoroutine(RunScene());
    }

    // Update is called once per frame
    void Update()
    {
      
        ResetUVs();
        next = false;
           
        Vector3 mousePos = Input.mousePosition;
        t.text = string.Format("{0:0.000}, {1:0.000}", mousePos.x / Screen.width, mousePos.y / Screen.height);

        if (Input.GetButtonDown("Jump"))
        {
            next = true;
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

    IEnumerator RunScene()
    {
        while (!next)// && !gesture.IsClap())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("starting chapel");
        float startTime = Time.time;
        while (Time.time - startTime < chapelFadeTime)
        {
            chapelMat.SetVector("_Value", new Vector4((Time.time - startTime) / chapelFadeTime, 0f, 0f, 0f));
            yield return null;
        }
        

        while (!next)// && !gesture.IsPsi())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("first branches");
        usingGrowth = true;
        screenModel.GetComponent<Renderer>().material = growMat;
        growMat.SetTexture("_MaskOneTex", MaskOneTex);
        growMat.SetTexture("_MaskTwoTex", MaskTwoTex);
        growMat.SetTexture("_MaskThreeTex", MaskThreeTex);
        growMat.SetTexture("_MaskFourTex", MaskFourTex);
        while (!next)
        {
            Blit();
            yield return null;
        }
        next = false;
        print("First words");
        SetMaskOne(.215f, .994f);
        yield return null;
        SetMaskOne(.222f, .942f);
        yield return null;
        SetMaskOne(.24f, .886f);
        yield return null;
        SetMaskOne(.58f, .065f);
        yield return null;
        SetMaskOne(.335f, .15f);
        while (!next)
        {
            Blit();
            yield return null;
        }
        next = false;
        print("central growth");
        SetMaskTwo(.425f, .01f);
        yield return null;
        SetMaskTwo(.446f, .01f);
        yield return null;
        SetMaskTwo(.553f, .01f);
        yield return null;
        SetMaskTwo(.574f, .01f);
        while (!next)
        {
            Blit();
            yield return null;
        }
        next = false;
        print("part 4 - growth from sides");
        SetMaskTwo(.005f, .01f);
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
        SetMaskOne(.965f, .01f);
        yield return null;
        SetMaskOne(.98f, .01f);
        yield return null;
        SetMaskOne(.999f, .01f);
        yield return null;
        SetMaskOne(.595f, .01f);
        yield return null;
        while (!next)
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
        while (!next)
        {
            Blit();
            yield return null;
        }
        next = false;
        print("Stage 5 fast");
        SetMaskOne(.275f, .005f);
        yield return null;
        SetMaskOne(.391f, .005f);
        yield return null;
        SetMaskTwo(.326f, .005f);
        yield return null;
        SetMaskTwo(.834f, .005f);
        yield return new WaitForSeconds(5f);
        print("Stage 5 fast - stage 2");
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
        while (!next)
        {
            Blit();
            yield return null;
        }
        next = false;
        print("third set of words");
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
        growMat.SetVector("_MaskThreeCoords", new Vector4(u, v, brushSize, -1f));
    }

    public void SetMaskFour(float u, float v)
    {
        growMat.SetVector("_MaskFourCoords", new Vector4(u, v, brushSize, -1f));
    }

    public void ResetUVs()
    {
        growMat.SetVector("_MaskOneCoords", new Vector4(-1, -1, -1, -1));
        growMat.SetVector("_MaskTwoCoords", new Vector4(-1, -1, -1, -1));
        growMat.SetVector("_MaskThreeCoords", new Vector4(-1, -1, -1, -1));
        growMat.SetVector("_MaskFourCoords", new Vector4(-1, -1, -1, -1));
    }

    IEnumerator EnableScreenCollision()
    {
        yield return new WaitForSeconds(5f);
        screenModel.GetComponent<MeshCollider>().enabled = true;
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
    }
